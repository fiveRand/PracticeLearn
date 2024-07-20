using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;
using static Inventory.Standard;

# if UNITY_EDITOR
using UnityEditor;
#endif

namespace Inventory
{

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemGrid))]
    public class ItemContainerEditor : Editor
    {
        ItemGrid targetScript;

        private void OnEnable()
        {
            targetScript = target as ItemGrid;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (GUILayout.Button("Open This Script"))
            {
                var monoScript = MonoScript.FromMonoBehaviour(targetScript);
                AssetDatabase.OpenAsset(monoScript);
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items"), true);

            EditorGUI.BeginChangeCheck();
            targetScript.width = EditorGUILayout.IntSlider("gridSizeX", targetScript.width, 1, 99);
            targetScript.height = EditorGUILayout.IntSlider("gridSizeY", targetScript.height, 1, 99);
            if (EditorGUI.EndChangeCheck())
            {
                var standard = targetScript.GetComponentInParent<Standard>();
                targetScript.SetGridSize(targetScript.width, targetScript.height, standard.tileUnit.tileSize);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
    public class ItemGrid : BaseGrid
    {
        public Item[] items;
        Item[,] itemSlots;

        protected override void Awake()
        {
            base.Awake();
            if (width == 0 || height == 0)
            {
                return;
            }
        }
        private void Start()
        {
            itemSlots = new Item[width, height];
            foreach (var item in items)
            {
                if(item == null)
                {
                    continue;
                }
                item.gameObject.SetActive(true);
                item.OnInitialize(this,transform);
                AutoInsert(item);
            }

        }

        public void AutoInsert(Item item)
        {
            for(int y =0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    Vector2Int index = new Vector2Int(x, y);
                    if (itemSlots[x,y] == null)
                    {
                        if (CanPlaceItem(item, x, y))
                        {
                            InsertItem(item, index, item.curAmount);
                            return;
                        }

                    }
                }
            }
        }

        bool CanPlaceItem(Item item,int startX,int startY)
        {
            int maxX = startX + item.width;
            int maxY = startY + item.height;

            if(maxX > width || maxY > height)
            // Check Boundary And auto rotate item to Fit
            {
                item.rotated = !item.rotated;

                maxX = startX + item.width;
                maxY = startY + item.height;

                if (maxX > width || maxY > height)
                {
                    item.rotated = !item.rotated;

                    return false;
                }
            }

            for (int x = startX; x < maxX; x++)
            {
                for(int y = startY; y < maxY; y++)
                {
                    if(itemSlots[x,y] != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public Item Test(Vector2Int index)
        {
            return itemSlots[index.x, index.y];
        }

        public void InsertItem(Item item, Vector2Int topLeftIndex, int amount = 1)
        {
            item.gameObject.SetActive(true);
            for (int x = topLeftIndex.x; x < topLeftIndex.x + item.width; x++)
            {
                for (int y = topLeftIndex.y; y < topLeftIndex.y + item.height; y++)
                {
                    itemSlots[x, y] = item;
                }
            }

            Vector2 position = standard.tileUnit.GetCenterPosition(item, topLeftIndex.x, topLeftIndex.y);
            item.rectTransform.localPosition = position;
            item.startX = topLeftIndex.x;
            item.startY = topLeftIndex.y;
        }

        public override void Delete(Item item)
        {

            for (int x = item.startX; x < item.startX + item.width; x++)
            {
                for (int y = item.startY; y < item.startY + item.height; y++)
                {
                    itemSlots[x, y] = null;
                }
            }
            standard.itemImagePooler.ReturnItem(item);
        }

        public void Add(Item placedItem,Item addingItem,int addAmount)
        {
            if(placedItem.itemSO != addingItem.itemSO)
            {
                Debug.LogError("PlacedItem and Adding Item are different! ");
                return;
            }

            int sum = placedItem.curAmount + addAmount;
            int leftover = placedItem.itemSO.maxAmount - sum;

            if (leftover < 0) // if sum is over maxAmount
            {
                int addNum = placedItem.itemSO.maxAmount - addAmount;

                placedItem.curAmount += addNum;
                addingItem.curAmount -= addNum;
                
            }
            else
            {
                placedItem.curAmount += addAmount;
                addingItem.curAmount -= addAmount;
            }
        }


        public Item GetItem(int x,int y)
        {
            if(!isIndexInBoundary(x,y))
            {
                return null;
            }

            var item = itemSlots[x, y];

            if(item == null)
            {
                return null;
            }
            return item;
        }


        public Item GetOverlapItem(Vector2Int topLeftIndex, Item pickedItem)
        {

            for (int x = topLeftIndex.x; x < topLeftIndex.x + pickedItem.width; x++)
            {
                for (int y = topLeftIndex.y; y < topLeftIndex.y + pickedItem.height; y++)
                {
                    if(itemSlots[x, y] != null)
                    {
                        return itemSlots[x, y];
                    }
                    
                }
            }
            return null;
        }






        public bool isIndexInBoundary(int x, int y)
        {
            return (x < 0 || y < 0 || x >= width || y >= height) == false;
        }

        public Vector2Int offsetBoundary(int x,int y,Item item)
        {
            Vector2Int result = new Vector2Int(x, y);

            // int offsetX = (item.width / 2 == 0) ? 

            // int minX = x

            int maxX = x + item.width;
            int maxY = y + item.height;
            
            if(x < 0)
            {
                result.x -= x;
            }
            else if(maxX >= width)
            {
                result.x -= maxX - width;
            }

            if(y < 0)
            {
                result.y += -y;
            }
            else if(maxY >= height)
            {
                result.y -= maxY - height;
            }

            return result;
        }

        /// <summary>
        /// it return slot index from mouse position
        /// 
        /// https://www.youtube.com/watch?v=2ajD1GDbEzA&ab_channel=GregDevStuff
        /// check 6:25 for specific direction.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public Vector2Int GetGridIndex(Vector2 mousePosition)
        {
            Vector2 positionOnTheGrid = new Vector2();
            positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
            positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;
            // Debug.Log($"MousePosition : {mousePosition} / rectTransform : {rectTransform.position} / result :  {positionOnTheGrid}");

            Vector2Int tileGridPosition = new Vector2Int();
            tileGridPosition.x = (int)(positionOnTheGrid.x / standard.tileUnit.interactTileSize.x);
            tileGridPosition.y = (int)(positionOnTheGrid.y / standard.tileUnit.interactTileSize.y);
            // Debug.Log($"Before : {positionOnTheGrid}, index : {tileGridPosition}");
            return tileGridPosition;
        }

        public override void SetPositionAndSize(Item item)
        {
            item.rectTransform.SetParent(this.rectTransform);
            item.rectTransform.anchorMax = new Vector2(0, 1);
            item.rectTransform.anchorMin = new Vector2(0, 1);
            item.rectTransform.pivot = Vector2.one * 0.5f;
            item.baseGrid = this;
        }
    }

}
