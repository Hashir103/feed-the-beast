using UnityEngine;
using Unity.Netcode;

public class ObjectGrabbable : NetworkBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    // Last known world pose when the client drops the object.
    private Vector3 lastDropPosition;
    private Quaternion lastDropRotation;
    private Vector3 lastDropVelocity;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }
    public void TryGrab(Transform grabPoint)
    {
        Debug.Log("[ObjectGrabbable] TryGrab requested by client for " + name);
        // Request the server to assign ownership
        RequestGrabServerRpc(NetworkManager.Singleton.LocalClientId);

        // Store who wants to grab it
        objectGrabPointTransform = grabPoint;

        if (objectRigidbody != null)
        {
            objectRigidbody.isKinematic = true;
            objectRigidbody.useGravity = false;
        }

        // Disable NetworkTransform while held so local motion isn't overridden
        var netTransform = GetComponent<Unity.Netcode.Components.NetworkTransform>();
        if (netTransform != null)
        {
            netTransform.enabled = false;
        }
    }

    public void TryDrop()
    {
        Debug.Log("[ObjectGrabbable] TryDrop requested by client for " + name);
        // Cache current pose so server can apply it authoritatively
        if (objectRigidbody != null)
        {
            lastDropPosition = transform.position;
            lastDropRotation = transform.rotation;
            // Start from rest; let gravity drive the fall
            lastDropVelocity = Vector3.zero;
        }

        RequestDropServerRpc(lastDropPosition, lastDropRotation, lastDropVelocity);
        objectGrabPointTransform = null;
        if (objectRigidbody != null)
        {
            objectRigidbody.isKinematic = false;
            objectRigidbody.useGravity = true;
        }

        // Re‑enable NetworkTransform so the dropped object syncs again
        var netTransform = GetComponent<Unity.Netcode.Components.NetworkTransform>();
        if (netTransform != null)
        {
            netTransform.enabled = true;
        }
    }

    #pragma warning disable 0618
    [ServerRpc(RequireOwnership = false)]
    private void RequestGrabServerRpc(ulong clientId)
    {
        Debug.Log("[ObjectGrabbable][ServerRpc] ChangeOwnership to client=" + clientId);
        // Server gives the client ownership of the object
        GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDropServerRpc(Vector3 dropPosition, Quaternion dropRotation, Vector3 dropVelocity)
    {
        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj == null || !netObj.IsSpawned) 
            return; 

        Debug.Log("[ObjectGrabbable][ServerRpc] Apply drop pose and RemoveOwnership for " + name);

        // Apply the client's final pose on the server so everyone sees the drop from the hand
        transform.position = dropPosition;
        transform.rotation = dropRotation;
        if (objectRigidbody != null)
        {
            objectRigidbody.linearVelocity = dropVelocity;
        }

        netObj.RemoveOwnership();
    }


    // ForceDrop item
    public void ForceDrop()
    {
        objectGrabPointTransform = null;
        if (objectRigidbody != null)
        {
            objectRigidbody.isKinematic = false;
            objectRigidbody.useGravity = true;
        }

        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj == null) return;

        if (IsServer)
        {
            Debug.Log("[ObjectGrabbable] Server force drop, clearing parent.");
            netObj.TrySetParent((Transform)null, false);
        }
        else
        {
            Debug.Log("[ObjectGrabbable] Client requests server force drop.");
            ForceDropServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ForceDropServerRpc()
    {
        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj != null)
        {
            Debug.Log("[ObjectGrabbable][ServerRpc] Clearing parent on server.");
            netObj.TrySetParent((Transform)null, false);
        }
    }
    #pragma warning restore 0618

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);
        }
    }
}
