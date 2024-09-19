using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemStore
{
    int AddItems(InventoryItem item, int number);
}
