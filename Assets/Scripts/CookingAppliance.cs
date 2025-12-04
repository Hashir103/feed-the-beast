using UnityEngine;
using Unity.Netcode;

public class CookingAppliance : NetworkBehaviour
{
    public FoodType[] acceptableFoods;

    private bool playerInRange = false;
    private PlayerPickupDrop player;
    [SerializeField] private string promptText = "Press 'C' to prepare";
    

    // Upon entering the appliance collider
    private void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<PlayerPickupDrop>();
        if (player != null && CanUseAppliance(player.heldFoodType))
        {
            other.GetComponent<PlayerPickupDrop>().nearbyAppliance = this;
            playerInRange = true;
            UpdatePrompt();
        }
    }

    // Upon exiting the appliance collider
    private void OnTriggerExit(Collider other)
    {
        if (player != null && other.GetComponent<PlayerPickupDrop>() == player)
        {
            playerInRange = false;
            other.GetComponent<PlayerPickupDrop>().nearbyAppliance = null;
            player = null;
            UIManager.Instance.HidePrompt();
        }
    }

    // Show or hide prompt to prepare
    private void UpdatePrompt()
    {
        if (player == null) return;
        if (playerInRange && player.IsHoldingPreparableItem())
        {
            UIManager.Instance.ShowPrompt(promptText);
        }
    }

    // Check if held food is valid for the appliance
    public bool CanUseAppliance(FoodType? held)
    {
        if (held == null) return false;

        foreach (var food in acceptableFoods)
        {
            if (food == held)
                return true;
        }
        return false;
    }

    public GameObject PrepareFood(GameObject rawItem, Transform grabPoint)
    {
        FoodItem food = rawItem.GetComponent<FoodItem>();
        if (food == null || food.preparedFood == null) 
            return null;

        // Force drop raw item before destroying
        ObjectGrabbable rawGrab = rawItem.GetComponent<ObjectGrabbable>();
        if (rawGrab != null)
            rawGrab.ForceDrop();

        // Destroy raw item
        Destroy(rawItem);

        // Get cooked food
        GameObject prepared = Instantiate(food.preparedFood, grabPoint.position, grabPoint.rotation);

        // Spawn
        NetworkObject netObj = prepared.GetComponent<NetworkObject>();
        if (netObj != null && !netObj.IsSpawned)
            netObj.Spawn();

        // Grab new cooked food object
        ObjectGrabbable grab = prepared.GetComponent<ObjectGrabbable>();
        if (grab != null)
            grab.TryGrab(grabPoint);

        return prepared;
    }
}
