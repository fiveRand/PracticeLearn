 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace Inventory
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif

#if UNITY_EDITOR
    [CustomEditor(typeof(Standard))]
    public class StandardEditor : Editor
    {

        Standard targetScript;

        private void OnEnable()
        {
            targetScript = target as Standard;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (GUILayout.Button("Open This Script"))
            {
                var monoScript = MonoScript.FromMonoBehaviour(targetScript);
                AssetDatabase.OpenAsset(monoScript);
            }

            GUI.enabled = false;
            targetScript.tileUnit.tileSize = EditorGUILayout.Vector2Field("tileSize", targetScript.tileUnit.tileSize);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("poolerParent"), true);
            targetScript.itemImagePooler.itemTemplate = (Item)EditorGUILayout.ObjectField(targetScript.itemImagePooler.itemTemplate, typeof(Item), true);
            var tileImage = (Texture2D)EditorGUILayout.ObjectField(targetScript.tileUnit.tileImage, typeof(Texture2D), true);
            if (tileImage != null)
            {
                Canvas canvas = targetScript.GetComponentInParent<Canvas>();
                var rectTransform = canvas.GetComponent<RectTransform>();
                targetScript.tileUnit.tileSize = new Vector2(tileImage.width, tileImage.height);
                targetScript.tileUnit.interactTileSize = new Vector2(tileImage.width * rectTransform.localScale.x, tileImage.height * rectTransform.localScale.y);
                targetScript.tileUnit.tileImage = tileImage;
            }
            serializedObject.ApplyModifiedProperties();
            EditorApplication.update.Invoke();

        }
    }
#endif



    /// <summary>
    /// this class contains about TileSize, ItemImage Template 
    /// </summary>
    public class Standard : MonoBehaviour
    {
        [System.Serializable]
        public class ItemImagePooler
        {
            public Transform poolerParent;
            public Item itemTemplate;
            ObjectPool<Item> pooler;

            public void Initialize(Transform poolerParent)
            {
                this.poolerParent = poolerParent;
                pooler = new ObjectPool<Item>(CreateItem, 10);
                pooler.OnReturnItemEvent += ReturnItem;
                pooler.OnTakeItemEvent += GetItem;
            }

            Item CreateItem()
            {
                Item item = Instantiate(itemTemplate, Vector3.zero, Quaternion.identity, poolerParent);
                item.gameObject.SetActive(false);
                return item;
            }

            public Item GetItem(Transform transform)
            {
                var newItem = pooler.Take();
                newItem.gameObject.SetActive(true);
                newItem.transform.SetParent(transform);
                newItem.transform.position = transform.position;
                newItem.transform.rotation = Quaternion.identity;
                return newItem;
            }

            public void ReturnItem(Item item)
            {

                item.gameObject.SetActive(false);
                item.transform.SetParent(poolerParent);
                pooler.Return(item);
            }
        }
        [System.Serializable]
        public class TileUnit
        {
            public Vector2 interactTileSize;
            public Vector2 tileSize;
            public Texture2D tileImage;

            public Vector2 GetCenterPosition(Item item, int startX, int startY)
            {
                float posX = startX * tileSize.x + tileSize.x * item.width * 0.5f;
                float posY = startY * tileSize.y + tileSize.y * item.height * 0.5f;

                return new Vector2(posX, -posY);
            }

            public Vector2 GetCenterPosition(Item item)
            {
                float posX = tileSize.x * item.width * 0.5f;
                float posY = tileSize.y * item.height * 0.5f;

                return new Vector2(posX, -posY);
            }

            public Vector2 GetItemSize(Item item)
            {
                float x = item.width * tileSize.x;
                float y = item.height * tileSize.y;
                return new Vector2(x, y);
            }
        }

        

        public static Standard instance;
        public Transform poolerParent;
        public ItemImagePooler itemImagePooler = new Standard.ItemImagePooler();
        public TileUnit tileUnit = new Standard.TileUnit();

        private void Awake() {
            instance = this;
            Canvas canvas = GetComponentInParent<Canvas>();
            itemImagePooler.Initialize(poolerParent);
            // SetCanvas();
        }


        void SetCanvas()
        {
            CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
            Canvas canvas = GetComponentInParent<Canvas>();

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                Debug.LogWarning($"Set {this.name}.Canvas renderMode to ScreenSpaceOverlay, setting to it.");
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            if (canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                Debug.LogWarning($"Set {this.name}.Canvas ScaleMode to ScaleWithScreenSize, also set Match to 0.5 ");
                Debug.LogWarning($"also, set Match to 0.5 and Reference PPU to 100 ");
                Debug.Log("canvasScaler have been adjusted.");
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.matchWidthOrHeight = 0.5f;
            }
        }
    }

}

