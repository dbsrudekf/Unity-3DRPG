using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pickup))]
public class ClickablePickup : MonoBehaviour, IRaycastable
{
    Pickup pickup;

    private void Awake()
    {
        pickup = GetComponent<Pickup>();
    }

    public CursorType GetCursorType()
    {
        if (pickup.CanBePickedUp())
        {
            return CursorType.PickUp;
        }
        else
        {
            return CursorType.FullPickup;
        }
    }

    public bool HandleRaycast(PlayerController callingController)
    {
        if (Input.GetMouseButtonDown(0))
        {
            pickup.PickupItem();
        }
        return true;
    }
}
