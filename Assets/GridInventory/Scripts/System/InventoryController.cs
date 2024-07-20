 using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Inventory
{
    
    public class InventoryController : MonoBehaviour
    {

        public BaseGrid baseGrid;
        public Vector2Int itemTopLeftIndex;
        InventoryHighlight highlight;
        public ForDraggingImage draggingImage;
        List<RaycastResult> result = new List<RaycastResult>();

        private void Awake()
        {
            highlight = GetComponentInChildren<InventoryHighlight>();
            
        }
        private void Update()
        {
            DetectBaseGridUpdate();
            draggingImage.transform.position = Input.mousePosition;
            OnHighLightUpdate();
            OnInputs();
        }

        void OnHighLightUpdate()
        {
            Vector2 mousePosition = Input.mousePosition;
            switch (baseGrid)
            {
                case ItemGrid grid:

                    mousePosition = draggingImage.GetCenterPositionFromItem(mousePosition);
  
                    itemTopLeftIndex = grid.GetGridIndex(mousePosition);
                    

                    if (draggingImage.DraggingItem != null)
                    {
                        itemTopLeftIndex = grid.offsetBoundary(itemTopLeftIndex.x, itemTopLeftIndex.y, draggingImage.DraggingItem);
                    }
                    highlight.OnHighLightUpdate(grid, draggingImage.DraggingItem, itemTopLeftIndex);
                    break;
                    case EquipmentSlot slot:
                    highlight.OnHighLightUpdate(slot, draggingImage.DraggingItem);
                    break;

                default:
                    highlight.SetSwitch(false);
                    break;
            }
        }


        void DetectBaseGridUpdate()
        {
            PointerEventData data = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            EventSystem.current.RaycastAll(data, result);
            if (result.Count > 0)
            {
                baseGrid = result[0].gameObject.GetComponentInChildren<BaseGrid>();
            }
            else
            {
                baseGrid = null;
            }
        }
        void OnInputs()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if(draggingImage.DraggingItem != null)
                {
                    draggingImage.OnRotate();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (draggingImage.DraggingItem == null) // if is empty handed
                {
                    var willbePickedItem = OnGetItem();
                    if (willbePickedItem == null)
                    {
                        return;
                    }

                    if(Input.GetKey(KeyCode.LeftControl))
                    {
                        OnPickUp(willbePickedItem, (int)(willbePickedItem.curAmount * 0.5f));
                    }
                    else if(Input.GetKey(KeyCode.LeftShift))
                    {
                        OnPickUp(willbePickedItem, 1);
                    }
                    else
                    {
                        OnPickUp(willbePickedItem, willbePickedItem.curAmount);
                    }
                }
                else
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        int amount = (int)(draggingImage.itemAmount * 0.5f);
                        if(amount < 1)
                        {
                            amount = 1;
                        }
                        OnDrop(draggingImage.DraggingItem, amount);
                    }
                    else if (Input.GetKey(KeyCode.LeftShift))
                    {
                        OnDrop(draggingImage.DraggingItem, 1);
                    }
                    else
                    {
                        OnDrop(draggingImage.DraggingItem, draggingImage.itemAmount);
                    }

                }
            }
            else if(Input.GetMouseButtonDown(1))
            {
                var item = OnGetItem() as IInteractableItem;
                if (item == null)
                {
                    return;
                }

                // item.OnInteract();
            }
        }

        Item OnGetItem()
        {
            switch (baseGrid)
            {
                case ItemGrid grid:
                    return grid.GetItem(itemTopLeftIndex.x, itemTopLeftIndex.y);
                    case EquipmentSlot slot:
                    return slot.equippedItem;
                default:
                    return null;
            }
        }

        void OnPickUp(Item willBePickedItem,int amount)
        {
            Item copyedItem = baseGrid.GetCopyItem(willBePickedItem, amount);
            willBePickedItem.curAmount -= amount;
            draggingImage.StartDrag(copyedItem);
        }

        void OnDrop(Item pickedItem,int amount)
        {
            switch (baseGrid)
            {
                case ItemGrid grid:
                    OnGridDrop(grid, pickedItem, amount);
                    break;

                    case EquipmentSlot slot:
                    OnSlotDrop(slot, pickedItem);
                    break;
                default:

                    break;
            }
        }
        void OnSlotDrop(EquipmentSlot slot,Item pickedItem)
        {
            if(!slot.CanEquip(pickedItem))
            {
                return;
            }

            Item overlapItem = slot.equippedItem;
            slot.OnEquip(pickedItem);
            draggingImage.itemAmount--;
            if (overlapItem != null)
            {
                draggingImage.StartDrag(overlapItem);
            }
        }


        void OnGridDrop(ItemGrid grid,Item pickedItem, int amount)
        {
            Item overlapItem = grid.GetOverlapItem(itemTopLeftIndex, pickedItem);
            draggingImage.itemAmount -= amount;
            if (overlapItem == null)
            {
                
                Item copyItem = grid.GetCopyItem(pickedItem, amount);
                grid.InsertItem(copyItem, itemTopLeftIndex,amount);
            }
            else // if swapping or adding
            {
                
                if(pickedItem.itemSO == overlapItem.itemSO && pickedItem.itemSO.maxAmount > 1)
                {
                    grid.Add(overlapItem, pickedItem,amount); 

                    return;
                }
                // if pickItem.size <= overlapItem.size
                else if(pickedItem.itemSO.width <= overlapItem.itemSO.width && pickedItem.itemSO.height <= overlapItem.itemSO.height && draggingImage.itemAmount == 0)
                {
                    Item tempOverlapItem = grid.GetCopyItem(overlapItem,overlapItem.curAmount);
                    grid.Delete(overlapItem);
                    grid.InsertItem(pickedItem,itemTopLeftIndex,amount);
                    draggingImage.StartDrag(tempOverlapItem);
                }
            } 
        }
    }

}
