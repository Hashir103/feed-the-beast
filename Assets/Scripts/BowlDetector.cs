using UnityEngine;
using Unity.Behavior;
using Unity.Netcode;

public class BowlDetector : NetworkBehaviour
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private string bowlVariableName = "Bowl";
    private BowlOwner bowlOwner;

    private void Awake()
    {
        bowlOwner = GetComponent<BowlOwner>();
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
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


        Debug.Log($"Food in {gameObject.name} (owned by {owner}): {foodName}");

        // agent.SetVariableValue(bowlVariableName + "Full", true);
        // agent.SetVariableValue(bowlVariableName + "FoodName", foodName);
    }
}
