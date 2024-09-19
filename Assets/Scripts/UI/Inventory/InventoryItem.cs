using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] string itemID = null;
    [SerializeField] string displayName = null;
    [SerializeField][TextArea] string description = null;
    [SerializeField] Sprite icon = null;
    [SerializeField] Pickup pickup = null;
    [SerializeField] bool stackable = false;
    [SerializeField] float price;
    [SerializeField] ItemCategory category = ItemCategory.None;

    static Dictionary<string, InventoryItem> itemLookupCache;

    public static InventoryItem GetFromID(string itemID)
    {
        if (itemLookupCache == null)
        {
            itemLookupCache = new Dictionary<string, InventoryItem>();
            var itemList = Resources.LoadAll<InventoryItem>("");
            foreach (var item in itemList)
            {
                if (itemLookupCache.ContainsKey(item.itemID))
                {
                    Debug.LogError(string.Format("Looks like there's a duplicate GameDevTV.UI.InventorySystem ID for objects: {0} and {1}", itemLookupCache[item.itemID], item));
                    continue;
                }

                itemLookupCache[item.itemID] = item;
            }
        }

        if (itemID == null || !itemLookupCache.ContainsKey(itemID)) return null;
        return itemLookupCache[itemID];
    }

    public Pickup SpawnPickup(Vector3 position, int number)
    {
        var pickup = Instantiate(this.pickup);
        pickup.transform.position = position;
        pickup.Setup(this, number);
        return pickup;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public string GetItemID()
    {
        return itemID;
    }

    public bool IsStackable()
    {
        return stackable;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public string GetDescription()
    {
        return description;
    }

    public float GetPrice()
    {
        return price;
    }

    public ItemCategory GetCategory()
    {
        return category;
    }


    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (string.IsNullOrWhiteSpace(itemID))
        {
            itemID = System.Guid.NewGuid().ToString();
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
    }
}
