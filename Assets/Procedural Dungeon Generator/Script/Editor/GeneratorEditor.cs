using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ProceduralDungeonGeneration
{
    [CustomEditor(typeof(Generator))]
    public class GeneratorEditor : Editor
    {
        Vector3 mouseDownPosition;
        Vector3 curMousePosition;
        Generator script;
        SerializedProperty a_;
        SerializedProperty b_;
        SerializedProperty corridorLength_;
        SerializedProperty rooms_;
        SerializedProperty roomAmount_;
        SerializedProperty placedRooms_;

        SerializedProperty wallTilemap_;
        SerializedProperty testTile_;
        private void OnEnable()
        {
            script = (Generator)target;

            a_ = serializedObject.FindProperty("a");
            b_ = serializedObject.FindProperty("b");
            rooms_ = serializedObject.FindProperty("rooms");
            roomAmount_ = serializedObject.FindProperty("roomAmount");
            placedRooms_ = serializedObject.FindProperty("placedRooms");
            corridorLength_ = serializedObject.FindProperty("corridorLength");
            wallTilemap_ = serializedObject.FindProperty("wallTilemap");
            testTile_ = serializedObject.FindProperty("testTile");
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ///
            /// serializedObject.Update 와 ApplyModifiedProperties는 한 몸이라고 생각하자
            /// Update = target(에디터로 사용하기로 정한 MonoBehaviour 스크립트 )을 재직렬화 해 최신 상태를 불러온다
            /// ApplyModifiedProperties = 글자 그대로 변경된 프로퍼티 값을 저장한다
            ///
            serializedObject.Update();

            if (GUILayout.Button("Test"))
            {
                script.Test();
            }

            if (GUILayout.Button("Generate"))
            {
                script.Generate();
            }
            if (GUILayout.Button("Clear"))
            {
                script.Clear();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            serializedObject.Update();

            OnMouseInput();
            Draw();

            serializedObject.ApplyModifiedProperties();
        }

        void Draw()
        {




            SceneView.RepaintAll();
        }

        void OnMouseInput()
        {
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlId);
            Event curEvent = Event.current;
            curMousePosition = Event.current.mousePosition;
            curMousePosition = HandleUtility.GUIPointToWorldRay(curMousePosition).origin;

            OnIdle();
        }

        void OnIdle()
        {
            Event curEvent = Event.current;


            if (curEvent.button == 0)
            {
                if (curEvent.type == EventType.MouseDown)
                {

                    mouseDownPosition = curMousePosition;

                }

            }
        }

    }

}
