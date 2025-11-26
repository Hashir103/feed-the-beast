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
    }

    public void TryDrop()
    {

        RequestDropServerRpc();
        objectGrabPointTransform = null;
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
        // Server clears ownership and returns it to server
        GetComponent<NetworkObject>().RemoveOwnership();
    }


    private void FixedUpdate()
    {
        // if (!IsOwner) return;
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);
        }
    }
}
