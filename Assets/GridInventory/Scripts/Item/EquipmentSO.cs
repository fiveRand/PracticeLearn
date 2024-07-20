using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;


namespace Inventory
{
    [CreateAssetMenu(menuName = "Item/Equipment")]
    public class EquipmentSO : ItemSO, IInteractableItem
    {
        public EquipmentType type;
        public int defenseAmount;

        public void OnInteract(PlayerStats stat)
        {
            stat.hatSprite.sprite = sprite;
            stat.defense += defenseAmount;
        }

        public void UnEquip(PlayerStats stat)
        {
            stat.hatSprite.sprite = null;
            stat.defense -= defenseAmount;
        }
    }

}