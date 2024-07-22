using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralDungeonGeneration
{
    [System.Serializable]
    public class Node : ScriptableObject
    {
        public enum Type
        {
            None,
            Spawn,
            Exit,
        }
        public List<Node> edges = new List<Node>();
        public Type type;
        public Vector2 position;
        public string title;


        public void Visualize()
        {
            title = EditorGUILayout.TextField("Title", title);
        }

        public void OnDelete(Node node)
        {

        }

        public Rect GetRect(Vector2 panOffset, float zoom)
        {
            var width = BaseData.DefaultWidth * zoom;
            var height = BaseData.DefaultHeight * zoom;

            return new Rect((position.x + panOffset.x) * zoom, (position.y + panOffset.y) * zoom, width, height);
        }

        /// <summary>
        /// Draws the room control.
        /// </summary>
        /// <remarks>
        /// It is advised to use the <see cref="GetRect"/> method for the base position and shape of the control.
        /// </remarks>
        /// <param name="gridOffset">Offset of the level graph editor window.</param>
        /// <param name="zoom">Zoom of the level graph editor window.</param>
        public void Draw(Vector2 gridOffset, float zoom)
        {
            // var style = Room.GetEditorStyle(IsSelected());

            var rect = GetRect(gridOffset, zoom);

            var oldBackgroundColor = GUI.backgroundColor;
            GUI.Box(rect, type.ToString());
            GUI.backgroundColor = oldBackgroundColor;
        }
    }

}
