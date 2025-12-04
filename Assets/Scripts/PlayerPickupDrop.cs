using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerPickupDrop : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickupLayerMask;
    [HideInInspector] public CookingAppliance nearbyAppliance;

    private ObjectGrabbable objectGrabbable;

    public FoodType? heldFoodType = null;
    public GameObject heldItem;
    

    private void Update()
    {
        if (!IsOwner) return; 

        // Pickup / drop food
        if (Keyboard.current[Key.E].wasPressedThisFrame)
        {
            // If not holding anything, try pick up
            if (objectGrabbable == null)
            {
                TryPickup();
            }
            else // If holding, drop
            {
                objectGrabbable.TryDrop();
                objectGrabbable = null;

                heldItem = null;
                heldFoodType = null;
            }
        }
        // Prepare food
        if (Keyboard.current[Key.C].wasPressedThisFrame)
        {
            if (objectGrabbable != null && nearbyAppliance != null)
            {
                GameObject raw = objectGrabbable.gameObject;
                FoodItem foodItem = raw.GetComponent<FoodItem>();
                if (foodItem == null) return;

                if (nearbyAppliance.CanUseAppliance(foodItem.foodType))
                {
                    GameObject prepared = nearbyAppliance.PrepareFood(raw, objectGrabPointTransform);

                    // Clear raw item
                    objectGrabbable = null;
                    heldItem = null;
                    heldFoodType = null;

                    // Get prepared item
                    if (prepared != null)
                    {
                        objectGrabbable = prepared.GetComponent<ObjectGrabbable>();
                        heldItem = prepared;
                        heldFoodType = FoodType.Prepared;
                    }
                }
            }
        }

    }

    private void TryPickup()
    {
        float pickupDistance = 2f;

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
            out RaycastHit hit, pickupDistance, pickupLayerMask))
        {
            if (hit.transform.TryGetComponent(out ObjectGrabbable grabbable))
            {
                objectGrabbable = grabbable;

                // Ask object to give us ownership and follow our grab point
                objectGrabbable.TryGrab(objectGrabPointTransform);

                heldItem = grabbable.gameObject;

                FoodItem food = grabbable.GetComponent<FoodItem>();
                heldFoodType = food != null ? food.foodType : (FoodType?)null;
            }
        }
    }

    // Check if player can cook the item being held
    public bool IsHoldingPreparableItem()
    {
        if (heldItem == null || heldItem.Equals(null)) return false;
        if (heldFoodType == FoodType.Prepared) return false;

        return heldItem.GetComponent<Preparable>() != null;
    }

    
}

