using Unity.Netcode;
using UnityEngine;
using Unity.Behavior;

public class BlackboardAssigner : NetworkBehaviour
{
    [SerializeField] private BehaviorGraphAgent agent;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponentInChildren<BehaviorGraphAgent>();

        agent.SetVariableValue<bool>("Bowl1Full", false);
        agent.SetVariableValue<bool>("Bowl2Full", false);
        Debug.Log("Startup agent instance: " + agent.GetInstanceID());
    }

    private void OnEnable()
    {
        TagAssignment.Player1Id.OnValueChanged += (_, _) => AssignPlayers();
        TagAssignment.Player2Id.OnValueChanged += (_, _) => AssignPlayers();
    }

    private void OnDisable()
    {
        TagAssignment.Player1Id.OnValueChanged -= (_, _) => AssignPlayers();
        TagAssignment.Player2Id.OnValueChanged -= (_, _) => AssignPlayers();
    }

    private void Start()
    {
        AssignPlayers();
    }

    private void AssignPlayers()
    {
        if (agent == null) return;

        var spawned = NetworkManager.Singleton.SpawnManager.SpawnedObjects;

        GameObject player1 = null;
        GameObject player2 = null;


        if (TagAssignment.Player1Id.Value != 0 && spawned.ContainsKey(TagAssignment.Player1Id.Value))
        {
            player1 = spawned[TagAssignment.Player1Id.Value].gameObject;
        }
        else
        {
            player1 = null;
        }


        if (TagAssignment.Player2Id.Value != 0 && spawned.ContainsKey(TagAssignment.Player2Id.Value))
        {
            player2 = spawned[TagAssignment.Player2Id.Value].gameObject;
        }
        else
        {
            player2 = null;
        }

        if (player1 != null)
            agent.SetVariableValue<GameObject>("Player1", player1);

        if (player2 != null)
            agent.SetVariableValue<GameObject>("Player2", player2);

        Debug.Log($"Blackboard assigned: P1={player1?.name}, P2={player2?.name}");
    }
}
