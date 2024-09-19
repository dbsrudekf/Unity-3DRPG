using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("GameDevTV/GameDevTV.UI.InventorySystem/Action Item"))]
public class ActionItem : InventoryItem
{
    [SerializeField] bool consumable = false;

    public virtual bool Use(GameObject user)
    {
        Debug.Log("Using action: " + this);
        return false;
    }

    public bool isConsumable()
    {
        return consumable;
    }
}
