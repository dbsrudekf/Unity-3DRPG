using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("GameDevTV/GameDevTV.UI.InventorySystem/Equipable Item"))]
public class EquipableItem : InventoryItem
{

    [SerializeField] EquipLocation allowedEquipLocation = EquipLocation.Weapon;
    [SerializeField] Condition equipCondition;

    public bool CanEquip(EquipLocation equipLocation, Equipment equipment)
    {
        if (equipLocation != allowedEquipLocation) return false;

        return equipCondition.Check(equipment.GetComponents<IPredicateEvaluator>());
    }

    public EquipLocation GetAllowedEquipLocation()
    {
        return allowedEquipLocation;
    }
}
