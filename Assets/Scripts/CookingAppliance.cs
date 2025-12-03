using UnityEngine;

public class CookingAppliance : MonoBehaviour
{
    private bool playerInRange = false;
    private PlayerInteraction player;

    [SerializeField] private string promptText = "Press 'C' to prepare";
    public FoodType[] acceptableFoods;

    private void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<PlayerInteraction>();
        if (player != null && CanUseAppliance(player.heldFoodType))
        {
            playerInRange = true;
            UpdatePrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (player != null && other.GetComponent<PlayerInteraction>() == player)
        {
            playerInRange = false;
            player.HidePrompt();
            player = null;
        }
    }

    private void UpdatePrompt()
    {
        if (player == null) return;
        if (playerInRange && player.IsHoldingPreparableItem())
        {
            player.ShowPrompt(promptText);
            Debug.Log(promptText);
        }


        Debug.Log("player is: " + player);
        Debug.Log("promptUI is: " + player?.promptUI);
    }

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
