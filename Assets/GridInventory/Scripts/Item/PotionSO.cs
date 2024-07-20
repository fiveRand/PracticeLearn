using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(menuName = "Item/Potion")]
    public class PotionSO : ItemSO,IInteractableItem
    {
        public int healAmount;

        public void OnInteract(PlayerStats stats)
        {
            stats.health += healAmount;
        }

    }
}
