using UnityEngine;
using Unity.Netcode;

public class ObjectGrabbable : NetworkBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

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
        objectRigidbody.isKinematic = true;
        if (objectRigidbody != null)
        {
            objectRigidbody.useGravity = false;
        }
    }

    public void TryDrop()
    {
        Debug.Log("[ObjectGrabbable] TryDrop requested by client for " + name);
        RequestDropServerRpc();
        objectGrabPointTransform = null;
        objectRigidbody.isKinematic = false;
        if (objectRigidbody != null)
        {
            objectRigidbody.useGravity = true;
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
    private void RequestDropServerRpc()
    {
        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj == null || !netObj.IsSpawned) 
            return; 

        Debug.Log("[ObjectGrabbable][ServerRpc] RemoveOwnership for " + name);
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
