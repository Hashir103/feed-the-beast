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
        // Request the server to assign ownership
        RequestGrabServerRpc(NetworkManager.Singleton.LocalClientId);

        // Store who wants to grab it
        objectGrabPointTransform = grabPoint;
        objectRigidbody.isKinematic = true;
    }

    public void TryDrop()
    {

        RequestDropServerRpc();
        objectGrabPointTransform = null;
        objectRigidbody.isKinematic = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestGrabServerRpc(ulong clientId)
    {
        // Server gives the client ownership of the object
        GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDropServerRpc()
    {
        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj == null || !netObj.IsSpawned) 
            return; 

        netObj.RemoveOwnership();
    }


    // ForceDrop item
    public void ForceDrop()
    {
        objectGrabPointTransform = null;
        if (objectRigidbody != null)
            objectRigidbody.isKinematic = false;
        transform.parent = null;
    }

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
