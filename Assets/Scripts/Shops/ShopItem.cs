using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    InventoryItem item;
    int availability;
    float price;
    int quantityInTransaction;

    public ShopItem(InventoryItem item, int availability, float price, int quantityInTransaction)
    {
        this.item = item;
        this.availability = availability;
        this.price = price;
        this.quantityInTransaction = quantityInTransaction;
    }

    public Sprite GetIcon()
    {
        return item.GetIcon();
    }

    public int GetAvailability()
    {
        return availability;
    }

    public string GetName()
    {
        return item.GetDisplayName();
    }

    public float GetPrice()
    {
        return price;
    }

    public InventoryItem GetInventoryItem()
    {
        return item;
    }

    public int GetQuantityInTransaction()
    {
        return quantityInTransaction;
    }
}
