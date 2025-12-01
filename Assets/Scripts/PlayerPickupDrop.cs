using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerPickupDrop : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickupLayerMask;

    private ObjectGrabbable objectGrabbable;

    private PlayerInteraction playerInteraction;

    private void Awake()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
        if (playerInteraction == null)
        {
            Debug.LogWarning("PlayerInteraction component not found on player!");
        }
    }

    private void Update()
    {
        if (!IsOwner) return; // Only the local player handles input

        if (Keyboard.current[Key.E].wasPressedThisFrame)
        {
            // If not holding anything → try pick up
            if (objectGrabbable == null)
            {
                TryPickup();
            }
            else // If holding → drop
            {
                objectGrabbable.TryDrop();
                objectGrabbable = null;

                // Update PlayerInteraction
                if (playerInteraction != null)
                {
                    playerInteraction.heldItem = null;
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

                // Update PlayerInteraction
                if (playerInteraction != null)
                {
                    playerInteraction.heldItem = grabbable.gameObject;
                }
            }
        }
    }
}

