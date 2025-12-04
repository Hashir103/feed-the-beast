using UnityEngine;
using Unity.Behavior;
using Unity.Netcode;

public class BowlDetector : NetworkBehaviour
{
    // [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private string bowlVariableName = "Bowl";
    private BowlOwner bowlOwner;
    private BehaviorGraphAgent agent;

    private void Awake()
    {
        bowlOwner = GetComponent<BowlOwner>();
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        agent = GameObject.Find("Creep1").GetComponent<BehaviorGraphAgent>();

        FoodItem food = other.GetComponent<FoodItem>();
        if (food == null) return;

        string foodName = food.gameObject.name;
        string owner;
        if (bowlOwner != null)
        {
            owner = bowlOwner.OwnerTag;
        }
        else
        {
            owner = null;
        }

        if (gameObject.name == "Bowl1")
        {
            agent.SetVariableValue<bool>("Bowl1Full", true);
        }
        else if (gameObject.name == "Bowl2")
        {
            agent.SetVariableValue<bool>("Bowl2Full", true);
        }
    }
}
