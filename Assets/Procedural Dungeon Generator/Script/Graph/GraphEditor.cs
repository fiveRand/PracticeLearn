using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ProceduralDungeonGeneration
{
    [CustomEditor(typeof(LevelGraphData))]
    public class GraphEditor : Editor
    {
        static LevelGraphData script;
        private void OnEnable() {
            script = target as LevelGraphData;
        }

        [OnOpenAsset]
        private static bool OpenAsset(int id,int line)
        {
            script = EditorUtility.InstanceIDToObject(id) as LevelGraphData;
            if(script != null)
            {
                var window = EditorWindow.GetWindow<GraphWindowEditor>("Graph Window Editor", typeof(SceneView));
                window.Show();
                window.Initialize(script);
                return true;   
            }
            return false;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if(GUILayout.Button("Open Graph Inspector"))
            {
                var monoScript = MonoScript.FromScriptableObject(this);
                AssetDatabase.OpenAsset(monoScript);
            }

            if(GUILayout.Button("Open Graph Editor"))
            {
                var window = EditorWindow.GetWindow<GraphWindowEditor>("Title", typeof(SceneView));
                window.Show();
                window.Initialize(target as LevelGraphData);
            }

            if (GUILayout.Button("Clear"))
            {
                script.Clear();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
