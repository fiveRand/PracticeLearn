using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
# if UNITY_EDITOR
using UnityEditor;
#endif
using static Inventory.Standard;

namespace Inventory
{
    public abstract class BaseSlotContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public int gridSizeX = 1;
        public int gridSizeY = 1;
        protected RectTransform rectTransform;
        public Standard standard;
        protected InventoryController controller;
        public abstract Item PickUp(InventoryController controller);
        public abstract bool Drop(InventoryController controller, Item imager, out Item overlapItem);
        protected virtual void Awake()
        {
            OnGetComponents();
            if (gridSizeX == 0 || gridSizeY == 0)
            {
                return;
            }

        }

        protected virtual void OnGetComponents()
        {
            controller = GetComponentInParent<InventoryController>();

            rectTransform = GetComponent<RectTransform>();
        }

        public void SetSize(int gridSizeX, int gridSizeY)
        {

            rectTransform = GetComponent<RectTransform>();


            float x = gridSizeX * standard.tileUnit.tileSize.x;
            float y = gridSizeY * standard.tileUnit.tileSize.y;

            Vector2 size = new Vector2(x, y);
            rectTransform.sizeDelta = size;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.pivot = Vector2.up;
        }
        public void OnPointerExit(PointerEventData eventData)
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {

        }
    }

}
