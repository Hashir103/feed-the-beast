using Unity.Netcode;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerCamera.enabled = true;
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
        }
    }
}
