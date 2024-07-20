using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory.Standard;

# if UNITY_EDITOR
using UnityEditor;
#endif


namespace Inventory
{

#if UNITY_EDITOR
    [CustomEditor(typeof(EquipmentSlot))]
    public class EquipmentSlotEditor : Editor
    {
        EquipmentSlot targetScript;

        private void OnEnable()
        {
            targetScript = target as EquipmentSlot;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (GUILayout.Button("Open This Script"))
            {
                var monoScript = MonoScript.FromMonoBehaviour(targetScript);
                AssetDatabase.OpenAsset(monoScript);
            }
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
    public class EquipmentSlot : BaseGrid
    {
        
        public Item equippedItem;
        public EquipmentType type;

        public void OnEquip(Item item)
        {
            item.gameObject.SetActive(true);
            SetPositionAndSize(item);
            equippedItem = item;
            adapter.OnUse(item.itemSO as EquipmentSO);
        }

        public bool CanEquip(Item item)
        {
            var equipment = item.itemSO as EquipmentSO;
            if(equipment == null || equipment.type != type)
            {
                return false;
            }
            return true;
        }

        public override void Delete(Item item)
        {
            adapter.UnEquip(item.itemSO as EquipmentSO);
            equippedItem = null;
            standard.itemImagePooler.ReturnItem(item);
            return;
        }

        public override void SetPositionAndSize(Item item)
        {
            item.rectTransform.SetParent(this.rectTransform);
            item.rectTransform.anchorMax = rectTransform.anchorMax;
            item.rectTransform.anchorMin = rectTransform.anchorMin;
            item.rectTransform.pivot = Vector2.zero;
            item.rectTransform.anchoredPosition = Vector2.zero;
            item.baseGrid = this;
        }

    }

}