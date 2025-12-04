using Unity.Netcode;
using UnityEngine;
using Unity.Behavior;

public class TagAssignment : NetworkBehaviour
{
    public static GameObject Player1;
    public static GameObject Player2;
    // [SerializeField] private BehaviorGraphAgent agent;
    // private string p1 = "Player1";
    // private string p2 = "Player2";

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
        {
            Player1 = gameObject;
            // agent.SetVariableValue<GameObject>(p1, gameObject);   
        }
        else if (tag == "P2")
        {
            Player2 = gameObject;
            // agent.SetVariableValue<GameObject>(p2, gameObject);
        }
    }
}
