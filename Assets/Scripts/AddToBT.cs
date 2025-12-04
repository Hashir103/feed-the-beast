using UnityEngine;
using Unity.Behavior;
using Unity.Netcode;

public class AddToBT : NetworkBehaviour
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private string p1 = "Player1";
    [SerializeField] private string p2 = "Player2";

    private void Awake()
    {

        bool b = agent.GetVariable("Player1", out BlackboardVariable v);

        if (!b)
        {
            agent.SetVariableValue<GameObject>(p1, gameObject);
            return;
        }
        else
        {
            agent.SetVariableValue<GameObject>(p2, gameObject);
            return;
        }
    }
}