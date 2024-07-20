using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory
{
    public abstract class BaseGrid : MonoBehaviour
    {
        [HideInInspector]
        public RectTransform rectTransform;
        protected InventoryController controller;
        protected Standard standard;
        protected PlayerAdapter adapter;
        [SerializeField]
        public int width = 1;
        [SerializeField]
        public int height = 1;
        protected virtual void Awake()
        {
            OnGetComponents();
        }

        protected virtual void OnGetComponents()
        {
            adapter = GetComponentInParent<PlayerAdapter>();
            controller = GetComponentInParent<InventoryController>();
            rectTransform = GetComponent<RectTransform>();
            standard = GetComponentInParent<Standard>();
        }

        public void SetGridSize(int width, int height, Vector2 tileSize)
        {
            rectTransform = GetComponent<RectTransform>();
            float x = width * tileSize.x;
            float y = height * tileSize.y;
            Vector2 size = new Vector2(x, y);
            rectTransform.sizeDelta = size;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.pivot = Vector2.up;
        }

        public abstract void SetPositionAndSize(Item item);

        public Item GetCopyItem(Item item, int amount = 1)
        {
            var tempItem = standard.itemImagePooler.GetItem(transform);
            tempItem.gameObject.SetActive(true);
            tempItem.itemSO = item.itemSO;
            tempItem.curAmount = amount;
            tempItem.OnInitialize(this, transform);

            return tempItem;
        }
        public abstract void Delete(Item item);
    }
}
