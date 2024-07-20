using System.Collections;
using System.Collections.Generic;
using static Inventory.Standard;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class Item : MonoBehaviour
    {
        public BaseGrid baseGrid;
        [SerializeField]ItemSO itemSO_;
        public ItemSO itemSO
        {
            get
            {
                return itemSO_;
            }
            set
            {
                itemSO_ = value;

                if(itemSO_ == null)
                {
                    return;
                }
                float x = itemSO_.width * instance.tileUnit.tileSize.x;
                float y = itemSO_.height * instance.tileUnit.tileSize.y;
                rectTransform.sizeDelta = new Vector2(x, y);
                rectTransform.localScale = Vector2.one;
                itemImage.sprite = itemSO.sprite;
            }
        }
        Image itemImage;
        Image backgroundImage;
        Text amountText;
        public int startX, startY;

        public int width
        {
            get
            {
                return (!rotated) ? itemSO.width : itemSO.height;
            }
        }

        public int height
        {
            get
            {
                return (!rotated) ? itemSO.height : itemSO.width;
            }
        }

        public RectTransform rectTransform;

        [SerializeField]
        protected bool rotated_;
        public bool rotated
        {
            get { return rotated_; }
            set
            {
                rotated_ = value;

                float angle = rotated_ ? 90f : 0f;
                rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        [SerializeField] int curAmount_;
        public int curAmount
        {
            get
            {
                return curAmount_;
            }
            set
            {
                curAmount_ = value;

                if (value <= 0)
                {
                    if(baseGrid != null)
                    {
                        baseGrid.Delete(this);
                    }
                }
                else if(value == 1)
                {
                    amountText.enabled = false;
                }
                else
                {
                    amountText.enabled = true;
                    amountText.text = value.ToString();
                }
            }
        }

        protected virtual void Awake()
        {
            amountText = GetComponentInChildren<Text>();
            rectTransform = GetComponent<RectTransform>();
            backgroundImage = GetComponentInChildren<Image>();
            itemImage = backgroundImage.transform.GetChild(0).GetComponent<Image>();
        }

        protected virtual void Start()
        {
            backgroundImage.enabled = false;
            itemImage.raycastTarget = false;
            itemImage.maskable = false;


            float x = width * instance.tileUnit.tileSize.x;
            float y = height * instance.tileUnit.tileSize.y;
            rectTransform.sizeDelta = new Vector2(x, y);
        }

        public void OnInitialize(BaseGrid grid,Transform gridTransform)
        {
            rectTransform.SetParent(gridTransform);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.pivot = Vector2.one * 0.5f;
            baseGrid = grid;
            this.itemSO = itemSO_;
            this.curAmount = curAmount_;
        }
    }

}
