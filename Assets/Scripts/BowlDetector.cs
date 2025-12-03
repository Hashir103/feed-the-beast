using UnityEngine;
using Unity.Behavior;
using Unity.Netcode;

public class BowlDetector : NetworkBehaviour
{
    [SerializeField] private BehaviorGraphAgent agent;

    [SerializeField] private string bowlVariableName = "Bowl";

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        FoodItem food = other.GetComponent<FoodItem>();
        string foodName = food.gameObject.name;
        Debug.Log($"Food in {gameObject.name}: {foodName}");

        // Set variables
        agent.SetVariableValue(bowlVariableName + "Full", true);
        agent.SetVariableValue(bowlVariableName + "FoodName", foodName);

    }
}
