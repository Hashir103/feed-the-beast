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
                Debug.Log($"[PlayerPickupDrop] C pressed; attempting cook. Held={objectGrabbable.name}, Appliance={nearbyAppliance.name}");
                GameObject raw = objectGrabbable.gameObject;
                FoodItem foodItem = raw.GetComponent<FoodItem>();
                if (foodItem == null) return;

                if (nearbyAppliance.CanUseAppliance(foodItem.foodType))
                {
                    NetworkObject applianceNet = nearbyAppliance.GetComponent<NetworkObject>();
                    NetworkObject rawNet = raw.GetComponent<NetworkObject>();
                    if (applianceNet != null && rawNet != null)
                    {
                        Debug.Log($"[PlayerPickupDrop] Sending RequestPrepareServerRpc. rawNetId={rawNet.NetworkObjectId}, applianceNetId={applianceNet.NetworkObjectId}");
                        RequestPrepareServerRpc(new NetworkObjectReference(applianceNet), new NetworkObjectReference(rawNet), objectGrabPointTransform.position, objectGrabPointTransform.rotation);
                        // Do not clear held item yet; wait for prepared receive
                    }
                    else
                    {
                        Debug.LogWarning("[PlayerPickupDrop] Missing NetworkObject on raw or appliance; cannot cook.");
                    }
                }
                else
                {
                    Debug.Log("[PlayerPickupDrop] Appliance cannot prepare this food type.");
                }
            }
            else
            {
                Debug.Log("[PlayerPickupDrop] C pressed but no held item or no appliance nearby.");
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

    public Transform GetGrabPoint()
    {
        return objectGrabPointTransform;
    }

    #pragma warning disable 0618
    [ServerRpc]
    private void RequestPrepareServerRpc(NetworkObjectReference applianceRef, NetworkObjectReference rawItemRef, Vector3 position, Quaternion rotation, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"[PlayerPickupDrop][ServerRpc] Received RequestPrepareServerRpc from client={serverRpcParams.Receive.SenderClientId}");
        // Resolve refs
        NetworkObject applianceNet;
        NetworkObject rawNet;
        if (!applianceRef.TryGet(out applianceNet) || !rawItemRef.TryGet(out rawNet)) return;

        var appliance = applianceNet.GetComponent<CookingAppliance>();
        if (appliance == null) return;

        // Perform server-side prepare
        Debug.Log($"[PlayerPickupDrop][ServerRpc] Preparing food on server. raw={rawNet.name} at pos={position}");
        GameObject prepared = appliance.PrepareFood(rawNet.gameObject, position, rotation);
        if (prepared == null) return;

        // Transfer ownership to requesting client
        var preparedNet = prepared.GetComponent<NetworkObject>();
        if (preparedNet != null)
        {
            Debug.Log($"[PlayerPickupDrop][ServerRpc] Changing ownership of prepared to client={serverRpcParams.Receive.SenderClientId}");
            preparedNet.ChangeOwnership(serverRpcParams.Receive.SenderClientId);
        }

        // Send reference back to requesting client to grab
        Debug.Log("[PlayerPickupDrop][ServerRpc] Sending ReceivePreparedClientRpc to requester.");
        ReceivePreparedClientRpc(new NetworkObjectReference(preparedNet), new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { serverRpcParams.Receive.SenderClientId } }
        });
    }

    [ClientRpc]
    private void ReceivePreparedClientRpc(NetworkObjectReference preparedRef, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("[PlayerPickupDrop][ClientRpc] Received prepared object reference.");
        NetworkObject netObj;
        if (!preparedRef.TryGet(out netObj)) { Debug.LogWarning("[PlayerPickupDrop][ClientRpc] Failed to resolve prepared NetworkObject."); return; }
        var prepared = netObj.gameObject;

        ObjectGrabbable grab = prepared.GetComponent<ObjectGrabbable>();
        if (grab != null)
        {
            Debug.Log($"[PlayerPickupDrop][ClientRpc] Grabbing prepared {prepared.name}.");
            // Snap to grab point immediately to avoid visual lag
            prepared.transform.position = objectGrabPointTransform.position;
            prepared.transform.rotation = objectGrabPointTransform.rotation;
            var rb = prepared.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            grab.TryGrab(objectGrabPointTransform);
            objectGrabbable = grab;
            heldItem = prepared;
            heldFoodType = FoodType.Prepared;
        }
        else
        {
            Debug.LogWarning("[PlayerPickupDrop][ClientRpc] Prepared object missing ObjectGrabbable.");
        }
    }
    #pragma warning restore 0618
    
}

