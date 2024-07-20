using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Inventory.Standard;

namespace Inventory
{
    public class ForDraggingImage : MonoBehaviour
    {
        InventoryController controller;
        RectTransform rectTransform;
        Image image;
        public Item DraggingItem;

        protected bool rotated_;
        public bool rotated
        {
            get { return rotated_; }
            set
            {
                rotated_ = value;

                float angle = rotated_ ? 90f : 0f;
                rectTransform.rotation = Quaternion.Euler(0, 0, angle);
                if(DraggingItem != null)
                {
                    DraggingItem.rotated = rotated;

                }
            }
        }

        int itemAmount_;
        public int itemAmount
        {
            get{ return itemAmount_; }
            set
            {
                itemAmount_ = value;

                if(itemAmount_ <= 0)
                {
                    EndDrag();
                }
            }
        }
        private void Awake() {
            controller = GetComponentInParent<InventoryController>();
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }


        public void OnRotate()
        {
            if (DraggingItem == null)
            {
                return;
            }
            rotated = !rotated;
        }


        public void StartDrag(Item DraggingItem)
        {
            this.DraggingItem = DraggingItem;
            this.itemAmount = DraggingItem.curAmount;
            image.enabled = true;
            image.sprite = DraggingItem.itemSO.sprite;
            rotated = DraggingItem.rotated;
            float x = this.DraggingItem.itemSO.width * instance.tileUnit.tileSize.x;
            float y = this.DraggingItem.itemSO.height * instance.tileUnit.tileSize.y;
            rectTransform.sizeDelta = new Vector2(x, y);
            DraggingItem.gameObject.SetActive(false);

        }

 

        public void EndDrag()
        {
            DraggingItem = null;
            image.enabled = false;
        }


        /// <summary>
        /// https://www.youtube.com/watch?v=2ajD1GDbEzA&ab_channel=GregDevStuff
        /// check 6:25 for specific direction.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public Vector2 GetCenterPositionFromItem(Vector2 mousePos)
        {

            if (DraggingItem != null)
            {
                var width = (!rotated) ? DraggingItem.itemSO.width : DraggingItem.itemSO.height;
                var height = (!rotated) ?DraggingItem.itemSO.height : DraggingItem.itemSO.width;
                mousePos.x -= (width - 1) *instance.tileUnit.interactTileSize.x * 0.5f;
                mousePos.y += (height - 1) *instance.tileUnit.interactTileSize.y * 0.5f;
            }

            return mousePos;
        }


    }

}