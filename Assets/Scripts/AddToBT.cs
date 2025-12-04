using UnityEngine;
using Unity.Behavior;
using Unity.Netcode;

public class AddToBT : NetworkBehaviour
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private string p1 = "Player1";
    [SerializeField] private string p2 = "Player2";


    // int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;


    private void Awake()
    {



        // int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        // Debug.Log("Players in game: " + playerCount);

        // if (playerCount == 1)
        // {
        //    agent.SetVariableValue(p1, gameObject);
        // } 
        // else
        // {
        //     agent.SetVariableValue(p2, gameObject);
        // }

        bool b = agent.GetVariable("Player1", out BlackboardVariable v);
        Debug.Log(b);

        if (!b)
        {
            Debug.Log("assigning mf");
            agent.SetVariableValue<GameObject>(p1, gameObject);
            bool b2 = agent.GetVariable("Player1", out BlackboardVariable v2);
            Debug.Log(b2);
            Debug.Log(v2);
            return;
        }
        else
        {
            Debug.Log("nope");
            agent.SetVariableValue<GameObject>(p2, gameObject);
            return;
        }
    }
}
