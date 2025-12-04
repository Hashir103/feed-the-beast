using Unity.Netcode;
using UnityEngine;

public class TagAssignment : NetworkBehaviour
{
    public static GameObject Player1;
    public static GameObject Player2;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        // Assign tags to spawning players (2 at most)
        if (NetworkManager.Singleton.ConnectedClients.Count == 1)
        {
            AssignTagServerRpc("P1");
        }
        else
        {
            AssignTagServerRpc("P2");
        }
    }

    [ServerRpc]
    private void AssignTagServerRpc(string tag)
    {
        gameObject.tag = tag;

        if (tag == "P1")
            Player1 = gameObject;
        else if (tag == "P2")
            Player2 = gameObject;
    }
}
