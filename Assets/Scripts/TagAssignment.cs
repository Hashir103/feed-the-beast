using Unity.Netcode;
using UnityEngine;

public class TagAssignment : NetworkBehaviour
{
    public static NetworkVariable<ulong> Player1Id = new NetworkVariable<ulong>(0);
    public static NetworkVariable<ulong> Player2Id = new NetworkVariable<ulong>(0);

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

    [ServerRpc(RequireOwnership = false)]
    private void AssignTagServerRpc(string tag)
    {
        gameObject.tag = tag;

        ulong netId = GetComponent<NetworkObject>().NetworkObjectId;

        if (tag == "P1") Player1Id.Value = netId;
        else if (tag == "P2") Player2Id.Value = netId;
    }
}
