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
            player = null;
            other.GetComponent<PlayerPickupDrop>().nearbyAppliance = null;
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
        else
        {
            UIManager.Instance.HidePrompt();
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

    public GameObject PrepareFood(GameObject rawItem, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        Debug.Log("[CookingAppliance] PrepareFood called on " + name + (IsServer ? " (server)" : " (not server)"));
        FoodItem food = rawItem.GetComponent<FoodItem>();
        if (food == null || food.preparedFood == null) 
        {
            Debug.LogWarning("[CookingAppliance] Invalid raw item or missing prepared prefab.");
            return null;
        }

        // Server-only execution; clients should request via player's ServerRpc
        if (!IsServer) { Debug.LogWarning("[CookingAppliance] PrepareFood must be called on server."); return null; }

        ObjectGrabbable rawGrab = rawItem.GetComponent<ObjectGrabbable>();
        if (rawGrab != null)
        {
            Debug.Log("[CookingAppliance] Force dropping raw item.");
            rawGrab.ForceDrop();
        }

        var rawNetObj = rawItem.GetComponent<NetworkObject>();
        if (rawNetObj != null && rawNetObj.IsSpawned)
        {
            Debug.Log("[CookingAppliance] Despawning raw NetworkObject.");
            rawNetObj.Despawn(true);
        }
        Debug.Log("[CookingAppliance] Destroying raw item GameObject.");
        Destroy(rawItem);

        Debug.Log($"[CookingAppliance] Instantiating prepared food at position={spawnPosition} rot={spawnRotation.eulerAngles}");
        GameObject prepared = Instantiate(food.preparedFood, spawnPosition, spawnRotation);

        // Preempt physics drop: start kinematic until client grabs
        var rb = prepared.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        NetworkObject netObj = prepared.GetComponent<NetworkObject>();
        if (netObj != null && !netObj.IsSpawned)
        {
            Debug.Log("[CookingAppliance] Spawning prepared NetworkObject.");
            netObj.Spawn();
        }

        UIManager.Instance.HidePrompt();
        // Re-evaluate prompt state after cooking; held item/state changed
        UpdatePrompt();
        Debug.Log("[CookingAppliance] Cooking complete.");

        return prepared;
    }

}
