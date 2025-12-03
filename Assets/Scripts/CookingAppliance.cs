using UnityEngine;

public class CookingAppliance : MonoBehaviour
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
}
