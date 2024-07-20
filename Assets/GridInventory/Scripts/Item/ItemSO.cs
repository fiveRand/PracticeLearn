using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory
{
    [CreateAssetMenu(menuName = "Item/Misc")]
    public class ItemSO : ScriptableObject
    {
        [Range(1, 10)]
        public int width = 1;
        [Range(1, 10)]
        public int height = 1;
        public Sprite sprite;

        public int maxAmount = 1;
    }
}