using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Inventory.Standard;

namespace Inventory
{
    [RequireComponent(typeof(Image))]
    public class InventoryHighlight : MonoBehaviour
    {
        RectTransform rectTransform;
        Image image;
        private void Reset()
        {
            image = GetComponent<Image>();
            image.enabled = false;
            image.color = new Color32(0, 0, 0, 150);
        }

        void Awake()
        {

            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
            image.color = new Color32(0, 0, 0, 150);
        }

        public void SetSwitch(bool turnOn)
        {
            image.enabled = turnOn;
        }

        public void OnHighLightUpdate(ItemGrid itemGrid,Item selectedItem, Vector2Int mouseStartIndex)
        {
            rectTransform.SetParent(itemGrid.transform);
            rectTransform.SetAsFirstSibling();
            if (selectedItem != null)
            {
                SetSwitch(true);
                rectTransform.sizeDelta = instance.tileUnit.GetItemSize(selectedItem);
                rectTransform.localPosition = instance.tileUnit.GetCenterPosition(selectedItem, mouseStartIndex.x, mouseStartIndex.y);
            }
            else
            {
                var item2Highlight = itemGrid.GetItem(mouseStartIndex.x, mouseStartIndex.y);
                
                bool canHighLight = item2Highlight != null;
                SetSwitch(canHighLight);
                if (canHighLight)
                {
                    rectTransform.sizeDelta = instance.tileUnit.GetItemSize(item2Highlight);
                    rectTransform.localPosition = instance.tileUnit.GetCenterPosition(item2Highlight, item2Highlight.startX, item2Highlight.startY);
                }
            }
        }

        public void OnHighLightUpdate(EquipmentSlot slot, Item pickedItem)
        {
            rectTransform.SetParent(slot.transform);
            rectTransform.SetAsFirstSibling();
            rectTransform.sizeDelta = slot.rectTransform.sizeDelta;
            rectTransform.anchoredPosition = Vector2.zero;
            if(pickedItem != null)
            {
                bool canEquip = slot.CanEquip(pickedItem);
                SetSwitch(canEquip);
            }
            else if(slot.equippedItem != null)
            {
                SetSwitch(true);
            }
        }

    }

}
