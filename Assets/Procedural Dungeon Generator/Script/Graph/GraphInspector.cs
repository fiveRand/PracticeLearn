using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ProceduralDungeonGeneration
{
    [CustomEditor(typeof(LevelGraphData))]
    public class GraphInspector : Editor
    {


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if(GUILayout.Button("Open Graph Inspector"))
            {
                var monoScript = MonoScript.FromScriptableObject(this);
                AssetDatabase.OpenAsset(monoScript);
            }

            if(GUILayout.Button("Open Graph Editor"))
            {
                var window = EditorWindow.GetWindow<GraphEditorWindow>("Title", typeof(SceneView));
                window.Show();
                window.Initialize(target as LevelGraphData);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
