using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edgar.Legacy.Utils;
using System.Linq;
using Edgar.Graphs;
using Edgar.Unity.Diagnostics;





#if UNITY_EDITOR_64
using UnityEditor;
#endif

namespace ProceduralDungeonGeneration
{
    public class GraphWindowEditor : EditorWindow
    {
        public enum Status
        {
            Normal,
            NodeDragging,
            ScreenDragging,
            Transitioning
        }
        Status status;
        Node spawnNode = null;
        Node exitNode = null;
        List<Node> nodes = new List<Node>();
        Vector2 panOffset = Vector2.zero;
        float zoom = 1;
        Node selectedNode;

        Vector2 mousePosition;

        LevelGraphData script;

        /*
        [MenuItem("Window/Node editor")]
        static void ShowEditor()
        {
            GraphEditorWindow editor = GetWindow<GraphEditorWindow>();
        }
        */

        public void OnEnable()
        {
            if(script != null)
            {
                Initialize(script);
            }
        }
        void OnGUI()
        {
            // Debug.Log(status);
            Inputs();
            Draws();

            // DrawGrid(16, 0.2f, Color.gray);
            // DrawGrid(16 * 5, 0.4f, Color.gray);
        }

        public void Clear()
        {
            nodes.Clear();

        }

        public void Initialize(LevelGraphData data)
        {
            this.script = data;
            nodes = new List<Node>(data.nodes);

            zoom = script.zoom;
            panOffset = script.panOffset;
            spawnNode = script.spawnNode;
            exitNode = script.exitNode;
            nodes = script.nodes;
        }

        string TrimPathForAssetDataBase(string folderPath)
        {
            int n = folderPath.IndexOf("Assets");
            string result = folderPath.Substring(n);
            return result;
        }

        string TrimSlash(string path)
        {
            // 
            // INPUT path = Assets/RandomFolderName/ScriptableObject.assets
            // RETURN path = Assets/RandomFolderName
            int rightIndex = path.LastIndexOf('/');
            string result = path.Substring(0,rightIndex);
            return result;
        }
        

        void Save()
        {


            script.panOffset = panOffset;
            script.zoom = zoom;

            // EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }

        void SaveAs()
        {
            string path = EditorUtility.OpenFolderPanel("Select file position", Application.dataPath, "");
            if (path.Length < 1)
            {
                Debug.Log("Canceled Save.");
                return;
            }
            path = TrimPathForAssetDataBase(path) + "/LevelGraphdata.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            script = ScriptableObject.CreateInstance<LevelGraphData>();

            AssetDatabase.CreateAsset(script, path);

            foreach (var node in nodes)
            {
                var temp = Instantiate<Node>(node);
                AssetDatabase.AddObjectToAsset(temp, script);
                if(node.type == Node.Type.Exit)
                {
                    script.exitNode = temp;
                }
                else if(node.type == Node.Type.Spawn)
                {
                    script.spawnNode = temp;
                }
                script.nodes.Add(temp);
            }
            script.zoom = zoom;
            script.panOffset = panOffset;

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = script;

        }


        void Load()
        {

            string path = EditorUtility.OpenFilePanel("Test", Application.dataPath, "asset");
            if(path.Length < 1)
            {
                return;
            }
            path = TrimPathForAssetDataBase(path);
            script = (LevelGraphData)AssetDatabase.LoadAssetAtPath(path, typeof(LevelGraphData));
            zoom = script.zoom;
            panOffset = script.panOffset;
            spawnNode = script.spawnNode;
            exitNode = script.exitNode;
            nodes = script.nodes;

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = script;
        }



        void Draws()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                node.Draw(panOffset, zoom);

                foreach(var other in node.edges)
                {
                    Rect start = node.GetRect(panOffset, zoom);
                    Rect end = other.GetRect(panOffset, zoom);

                    VisualizeConnection(start, end);
                }
            }
        }

        void OnEventMouseDrag()
        {
            Event curEvent = Event.current;
            if (status == Status.NodeDragging)
            {
                var dragOffset = Event.current.delta / zoom;
                selectedNode.position += dragOffset;
            }
            else if (status == Status.ScreenDragging)
            {
                panOffset += curEvent.delta / zoom;
            }
        }


        void Inputs()
        {

            Event curEvent = Event.current;
            mousePosition = curEvent.mousePosition;

            switch (curEvent.type)
            {
                case EventType.MouseDown:
                    OnEventMouseDown();
                    break;

                case EventType.MouseDrag:
                    OnEventMouseDrag();
                    break;
                case EventType.MouseUp:
                    OnEventMouseUp();
                    break;

                case EventType.KeyDown:
                    if (curEvent.keyCode == KeyCode.PageUp)
                    {
                        Zoom(1);
                    }
                    else if (curEvent.keyCode == KeyCode.PageDown)
                    {
                        Zoom(-1);
                    }
                    break;

                case EventType.ScrollWheel:

                    Zoom(curEvent.delta.y);
                    break;
            }

            if (status == Status.Transitioning)
            {
                Rect mouseRect = new Rect(mousePosition, Vector2.one);
                var selectedNodeRect = selectedNode.GetRect(panOffset, zoom);
                VisualizeConnection(selectedNodeRect, mouseRect);
            }
            GUI.changed = true;
        }

        void Zoom(float y)
        {

            Event e = Event.current;
            var oldZoom = zoom;

            if (y > 0)
            {
                zoom -= zoom * 0.1f;
            }
            else
            {
                zoom += zoom * 0.1f;
            }

            // Zoom value must be in the interval [0.1,5]
            zoom = Mathf.Max(0.1f, zoom);
            zoom = Mathf.Min(5, zoom);

            // If zoom center is the current mouse position, zoom will focus on that position
            // An alternative is to use position.size / 2 in which case the center of the window will be the same after zooming
            var zoomCenter = e.mousePosition;

            // This equation makes sure that zoom center is the focus of the zoom
            panOffset += -(zoom * (zoomCenter - panOffset * oldZoom) - zoomCenter * oldZoom) / (zoom * oldZoom) - panOffset;

        }

        void OnEventMouseUp()
        {
            int selectedNodeIndex = GetNodeFromMousePosition();
            Event curEvent = Event.current;

            if (curEvent.button == 0)
            {
                if (status == Status.NodeDragging || status == Status.ScreenDragging)
                {
                    status = Status.Normal;
                }
            }

        }
        void OnEventMouseDown()
        {
            int selectedNodeIndex = GetNodeFromMousePosition();
            Event curEvent = Event.current;



            if (curEvent.button == 0)
            {
                if (status == Status.Normal)
                {
                    if (selectedNodeIndex == -1)
                    {
                        status = Status.ScreenDragging;
                    }
                    else
                    {
                        selectedNode = nodes[selectedNodeIndex];
                        status = Status.NodeDragging;
                    }
                }
                else if (status == Status.Transitioning)
                {
                    script.AddUndirectedEdge(selectedNode, nodes[selectedNodeIndex]);
                    status = Status.Normal;
                    selectedNode = null;
                }
            }

            else if (curEvent.button == 1)
            {
                if (status == Status.Normal)
                {
                    GenericMenu menu = new GenericMenu();
                    if (selectedNodeIndex == -1)
                    {
                        menu.AddItem(new GUIContent("Room/" + Node.Type.Spawn.ToString()), false, RoomCallBack, Node.Type.Spawn);
                        menu.AddItem(new GUIContent("Room/" + Node.Type.None.ToString()), false, RoomCallBack, Node.Type.None);
                        menu.AddItem(new GUIContent("Room/" + Node.Type.Exit.ToString()), false, RoomCallBack, Node.Type.Exit);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Save"), false, WindowCallBack, "Save");
                        menu.AddItem(new GUIContent("SaveAs"), false, WindowCallBack, "SaveAs");
                        menu.AddItem(new GUIContent("Load"), false, WindowCallBack, "Load");
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Make Transition"), false, ControlCallBack, "Transition");
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Delete Room"), false, ControlCallBack, "Delete");

                    }
                    menu.ShowAsContext();
                    curEvent.Use();
                }
                else if(status == Status.Transitioning)
                {
                    status = Status.Normal;
                }
            }
        }

        void RoomCallBack(object callback)
        {
            Node.Type type = (Node.Type)callback;

            var newnode = CreateNode(type);

            if(type == Node.Type.Spawn)
            {
                if(spawnNode!=null)
                {
                    spawnNode.type = Node.Type.None;
                }
                spawnNode = newnode;
            }
            else if(type == Node.Type.Exit )
            {
                if (exitNode != null)
                {
                    exitNode.type = Node.Type.None;
                }

                exitNode = newnode;
            }
            
            nodes.Add(newnode);

        }

        Node CreateNode(Node.Type type)
        {
            Vector2 roomPosition = WorldToGridPosition(mousePosition);

            var path = AssetDatabase.GetAssetPath(script);
            var folderPath = TrimSlash(path);

            // bool isAssetExist = AssetDatabase.GetMainAssetTypeAtPath(script.nodesFolderPath) != null;

            if (!AssetDatabase.IsValidFolder(script.nodesFolderPath))
            {
                folderPath = AssetDatabase.CreateFolder(folderPath, script.name + "'s nodes");
                // it return GUID!
                folderPath = AssetDatabase.GUIDToAssetPath(folderPath);
                script.nodesFolderPath = folderPath;
            }
            else
            {
                folderPath = script.nodesFolderPath;
            }


            Node newNode = CreateInstance<Node>();
            newNode.position = roomPosition;
            newNode.type = type;
            var nodePath = $"{folderPath}/{newNode.type.ToString()}.asset";
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath(nodePath);
            AssetDatabase.CreateAsset(newNode, uniquePath);

            if (newNode.type == Node.Type.Spawn)
            {
                script.spawnNode = newNode;
            }
            else if (newNode.type == Node.Type.Exit)
            {
                script.exitNode = newNode;
            }
            return newNode;
        }

        /// <summary>
        /// Transforms a given world (mouse) position to a local grid position, taking into account pan offset and zoom.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector2 WorldToGridPosition(Vector2 position)
        {
            return (position - panOffset * zoom) / zoom;
        }

        void WindowCallBack(object callback)
        {
            string name = callback.ToString();
            switch (name)
            {

                case "Save":
                    Save();
                    break;
                case "SaveAs":
                    SaveAs();
                    break;
                case "Load":
                    Load();
                    break;
            }
        }

        void ControlCallBack(object callback)
        {
            string name = callback.ToString();
            int index = GetNodeFromMousePosition();
            switch (name)
            {
                case "Delete":
                    if (index == -1)
                    {
                        return;
                    }
                    selectedNode = nodes[index];
                    foreach(var node in nodes)
                    {
                        node.edges.Remove(selectedNode);
                        selectedNode.edges.Remove(node);
                    }
                    nodes.Remove(selectedNode);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedNode));
                    break;
                case "Transition":

                    if (index != -1)
                    {
                        selectedNode = nodes[index];
                        status = Status.Transitioning;
                    }

                    break;
            }
        }

        int GetNodeFromMousePosition()
        {

            int selectedIndex = -1;

            for (int i = 0; i < nodes.Count; i++)
            {
                Rect nodeRect = nodes[i].GetRect(panOffset, zoom);

                if (nodeRect.Contains(mousePosition))
                {
                    selectedIndex = i;
                    break;
                }
            }
            return selectedIndex;
        }

        public void VisualizeConnection(Rect start, Rect end)
        {

            Handles.color = Color.black;
            Handles.DrawLine(start.center, end.center);

            Handles.color = Color.white;

        }

        /// <summary>
        /// Draws the grid.
        /// </summary>
        /// <param name="gridSpacing"></param>
        /// <param name="gridOpacity"></param>
        /// <param name="gridColor"></param>
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            gridSpacing = gridSpacing * zoom;

            var widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            var heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            var originalHandleColor = Handles.color;
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            var newOffset = new Vector3((zoom * panOffset.x) % gridSpacing, (zoom * panOffset.y) % gridSpacing, 0);

            // Draw vertical lines
            for (var i = 0; i < widthDivs + 1; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height + gridSpacing, 0f) + newOffset);
            }

            // Draw horizontal lines
            for (var j = 0; j < heightDivs + 1; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width + gridSpacing, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = originalHandleColor;
            Handles.EndGUI();
        }

    }

}
