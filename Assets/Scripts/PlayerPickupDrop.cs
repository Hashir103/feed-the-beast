using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerPickupDrop : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickupLayerMask;

    private ObjectGrabbable objectGrabbable;
    private void Update()
    {
        if (Keyboard.current[Key.E].isPressed)
        {
        // if (!IsOwner) return;    // Only the local player handles input

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
            }
        }
    }
}
