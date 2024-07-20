using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlasticGui.WorkspaceWindow.Items;
using Edgar.Legacy.Utils;
using System.Linq;



#if UNITY_EDITOR_64
using UnityEditor;
#endif

namespace MyGraph
{
    public class GraphEditorWindow : EditorWindow
    {
        public enum Status
        {
            Normal,
            NodeDragging,
            ScreenDragging,
            Transitioning
        }
        Status status;
        List<Node> nodes = new List<Node>();
        List<Edge> edges = new List<Edge>();
        Vector2 panOffset = Vector2.zero;
        float zoom = 1;
        Node selectedNode;

        Vector2 mousePosition;

        LevelGraphData data;

        /*
        [MenuItem("Window/Node editor")]
        static void ShowEditor()
        {
            GraphEditorWindow editor = GetWindow<GraphEditorWindow>();
        }
        */

        public void OnEnable()
        {
            if(data != null)
            {
                Initialize(data);
            }
        }

        public void Initialize(LevelGraphData data)
        {
            this.data = data;
            nodes = new List<Node>(data.nodes);
            edges = new List<Edge>(data.edges);

            zoom = data.zoom;
            panOffset = data.panOffset;
        }

        string TrimPathForAssetDataBase(string folderPath)
        {
            int n = folderPath.IndexOf("Assets");
            string result = folderPath.Substring(n);
            return result;
        }
        

        void Save()
        {

            foreach (var node in nodes)
            {
                if(!AssetDatabase.Contains(node.GetInstanceID()))
                {
                    AssetDatabase.AddObjectToAsset(node, data);
                    data.nodes.Add(node);
                }
            }

            foreach(var edge in edges)
            {
                if (!AssetDatabase.Contains(edge.GetInstanceID()))
                {
                    AssetDatabase.AddObjectToAsset(edge, data);
                    data.edges.Add(edge);
                }
            }


        

            data.panOffset = panOffset;
            data.zoom = zoom;
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

            data = ScriptableObject.CreateInstance<LevelGraphData>();

            AssetDatabase.CreateAsset(data, path);

            foreach (var node in nodes)
            {
                var temp = Instantiate<Node>(node);
                AssetDatabase.AddObjectToAsset(temp, data);

                data.nodes.Add(temp);
            }
            data.zoom = zoom;
            data.panOffset = panOffset;

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = data;

        }


        void Load()
        {

            string path = EditorUtility.OpenFilePanel("Test", Application.dataPath, "asset");
            if(path.Length < 1)
            {
                return;
            }
            path = TrimPathForAssetDataBase(path);
            data = (LevelGraphData)AssetDatabase.LoadAssetAtPath(path, typeof(LevelGraphData));
            zoom = data.zoom;
            panOffset = data.panOffset;
            nodes = data.nodes;
            edges = data.edges;
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = data;
        }

        void OnGUI()
        {
            // Debug.Log(status);
            Inputs();
            Draws();

            // DrawGrid(16, 0.2f, Color.gray);
            // DrawGrid(16 * 5, 0.4f, Color.gray);
        }

        void Draws()
        {
            foreach (Edge e in edges)
            {
                Rect start = e.start.GetRect(panOffset, zoom);
                Rect end = e.end.GetRect(panOffset, zoom);
                VisualizeConnection(start, end);
            }
            
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw(panOffset, zoom);
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
                    Edge newEdge = CreateInstance<Edge>();
                    {
                        newEdge.start = selectedNode;
                        newEdge.end = nodes[selectedNodeIndex];
                    }

                    edges.Add(newEdge);

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
                        menu.AddItem(new GUIContent("Room/Start"), false, RoomCallBack, "Start");
                        menu.AddItem(new GUIContent("Room/Normal"), false, RoomCallBack, "Normal");
                        menu.AddItem(new GUIContent("Room/End"), false, RoomCallBack, "End");
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
            }
        }

        void RoomCallBack(object callback)
        {
            string name = callback.ToString();

            Vector2 roomPosition = WorldToGridPosition(mousePosition);
            Node newNode = CreateInstance<Node>();
            newNode.position = roomPosition;
            switch (name)
            {
                case "Start":

                    break;
                    case "End":

                    break;
                    case "Normal":

                    break;
            }
            nodes.Add(newNode);
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
                    Node node = nodes[index];
                    nodes.RemoveAt(index);
                    Queue<Edge> willbeDeletedEdge = new Queue<Edge>();
                    foreach (Edge e in edges)
                    {
                        if (e.start == node || e.end == node)
                        {
                            willbeDeletedEdge.Enqueue(e);
                        }
                    }

                    while (willbeDeletedEdge.Count > 0)
                    {
                        var edge = willbeDeletedEdge.Dequeue();
                        edges.Remove(edge);
                    }

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
