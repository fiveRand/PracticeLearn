using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

public class PlayerAdapter : MonoBehaviour
{
    public PlayerStats player;

    public void OnUse(IInteractableItem item)
    {
        item.OnInteract(player);
    }
    public void UnEquip(EquipmentSO equipment)
    {
        equipment.UnEquip(player);
    }

}
