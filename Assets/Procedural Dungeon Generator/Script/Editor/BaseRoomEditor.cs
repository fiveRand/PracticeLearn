using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(BaseRoom))]
public class BaseRoomInspector : Editor
{

    public enum Mode
    {
        Idle,
        Add,
        Delete,
        Clear
    }
    BaseRoom script;
    Mode curMode;
    string[] gridName = { Mode.Idle.ToString(), Mode.Add.ToString(), Mode.Delete.ToString(), Mode.Clear.ToString() };

    Vector2 curMousePosition;
    Vector3 mouseDownPosition;
    bool isDragging = false;
    Grid grid;


    

    bool showCollisionRect = false;
    private void OnEnable() {
        curMode = Mode.Idle;
        script = (BaseRoom)target;
        grid = script.GetComponentInChildren<Grid>();

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        curMode = (Mode)GUILayout.SelectionGrid((int)curMode, gridName, 3);

        if(GUILayout.Button("Resize Collsion Rect"))
        {
            script.ResetCollisionRect();
            EditorUtility.SetDirty(target);
        }


        
        showCollisionRect = GUILayout.Toggle(showCollisionRect, "Show Collision Rect");


        
        serializedObject.ApplyModifiedProperties();


    }

    void DrawDoors()
    {
        foreach(var door in script.doors)
        {
            GetBLTR(door.start, door.end, out Vector3Int doorBL, out Vector3Int doorTR);
            DrawRectangle(doorBL, doorTR, Color.cyan);
            if (door.clearAmount > 0)
            {

                GetBLTR(door.start, door.start - (door.dir.Vectorize() * door.clearAmount), out Vector3Int clearBL, out Vector3Int clearTR);
                DrawRectangle(clearBL, clearTR, Color.red);
            }

        }
    }

    void DrawRectangle(Vector3Int bottomLeft,Vector3Int topRight,Color outlineColor )
    {
        topRight += new Vector3Int(1, 1, 0);
        Vector3[] vertices = GetOutlineVertices(bottomLeft, topRight);
        Handles.DrawSolidRectangleWithOutline(vertices, Color.clear, outlineColor);
    }

    void DrawCollisionRect()
    {
        Color halfTransparent = new Color(1, 1, 1, 0.3f);

        Vector3[] array = new Vector3[4]
        {
            new Vector3(script.collisionRect.xMin, script.collisionRect.yMin, 0f),
            new Vector3(script.collisionRect.xMax, script.collisionRect.yMin, 0f),
            new Vector3(script.collisionRect.xMax, script.collisionRect.yMax, 0f),
            new Vector3(script.collisionRect.xMin, script.collisionRect.yMax, 0f)
        };

        Handles.DrawSolidRectangleWithOutline(array, halfTransparent, Color.black);
    }


    private void OnSceneGUI() {
        var controlId = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(controlId);

        DrawDoors();
        if(showCollisionRect)
        {
            DrawCollisionRect();
        }
        switch (curMode)
        {
            case Mode.Idle:
                break;

            case Mode.Add:
                OnAddDoor();
                break;
                case Mode.Delete:
                OnDeleteDoor();
                break;
                case Mode.Clear:
                OnClear();
                break;
        }
        curMousePosition = Event.current.mousePosition;
        if (Event.current.button == 0)
        {
            if (Event.current.type == EventType.MouseDown)
            {

                mouseDownPosition = curMousePosition;
                isDragging = !isDragging;
            }

        }
        else if (Event.current.button == 1)
        {
            isDragging = false;
        }

        SceneView.RepaintAll();
    }

    void OnChaining()
    {

        if(isDragging)
        {

            var mouseDownIndex = GetIndex(mouseDownPosition);
            var curMouseIndex = GetIndex(curMousePosition);
            GetBLTR(mouseDownIndex, curMouseIndex, out Vector3Int bottomLeft, out Vector3Int topRight);
            topRight += new Vector3Int(1, 1, 0);
            Vector3[] vertices = GetOutlineVertices(bottomLeft, topRight);
            Handles.DrawSolidRectangleWithOutline(vertices, Color.clear, Color.red);


            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {

                isDragging = !isDragging;
            }

        }
        else
        {
            DrawTile(curMousePosition, Color.red);
        }
    }

    public void GetBLTR(Vector3Int a, Vector3Int b,out Vector3Int bottomLeft,out Vector3Int topRight)
    {
        Vector3[] vertices = new Vector3[4];

        bottomLeft = Vector3Int.zero;
        topRight = Vector3Int.zero;

        if (a.x <= b.x)
        {
            bottomLeft.x = a.x;
            topRight.x = b.x;
        }
        else
        {
            bottomLeft.x = b.x;
            topRight.x = a.x;
        }

        if (a.y <= b.y)
        {
            bottomLeft.y = a.y;
            topRight.y = b.y;
        }
        else
        {
            bottomLeft.y = b.y;
            topRight.y = a.y;
        }
    }

    public Vector3[] GetOutlineVertices(Vector3Int bottomLeft, Vector3Int topRight)
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = bottomLeft;
        vertices[1] = new Vector3(bottomLeft.x, topRight.y);
        vertices[2] = topRight;
        vertices[3] = new Vector3(topRight.x, bottomLeft.y);
        return vertices;

    }


    void OnClear()
    {
        script.doors.Clear();
    }

    void DrawTile(Vector2 position, Color color)
    {
        Vector3Int tileIndex = GetIndex(position);
        Vector3 cellPosition = grid.GetCellCenterWorld(tileIndex);
        Handles.color = color;
        Handles.DrawWireCube(cellPosition, Vector3.one);
        Handles.color = Color.white;
    }



    Vector3Int GetIndex(Vector2 position)
    {
        Vector3 worldPosition = HandleUtility.GUIPointToWorldRay(position).origin;
        Vector3Int tileIndex = grid.WorldToCell(worldPosition);
        return tileIndex;
    }


    void OnAddDoor()
    {
        Event curEvent = Event.current;
        Vector2 curMousePosition = Event.current.mousePosition;

        if (isDragging)
        {
            // Handles.BeginGUI();
            float distX = Mathf.Abs(curMousePosition.x - mouseDownPosition.x);
            float dirX = Mathf.Sign(curMousePosition.x - mouseDownPosition.x);
            float distY = Mathf.Abs(curMousePosition.y - mouseDownPosition.y);
            float dirY = Mathf.Sign(curMousePosition.y - mouseDownPosition.y);

            Vector2 nextPosition = mouseDownPosition;
            nextPosition += (distX > distY) ? Vector2.right * dirX*distX : Vector2.up * dirY*distY;
            DrawTile(mouseDownPosition, Color.green);
            DrawTile(nextPosition, Color.green);

            if(curEvent.type == EventType.MouseDown && curEvent.button == 0)
            {
                bool isX = distX > distY;
                var start = GetIndex(mouseDownPosition);
                var end = GetIndex(nextPosition);
                

                Door2D newDoor = new Door2D(start, end, isX,script);
                Undo.RecordObject(script, "Added Door");
                script.doors.Add(newDoor);
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(script);


            }


            // Handles.EndGUI();
        }
        else
        {

            DrawTile(curMousePosition, Color.green);
        }
    }

    void OnDeleteDoor()
    {
        DrawTile(curMousePosition, Color.red);
        if (Event.current.button == 0)
        {
            if(Event.current.type == EventType.MouseDown)
            {
                var mouseDownIndex = GetIndex(mouseDownPosition);

                for (int i = 0; i < script.doors.Count; i++)
                {
                    var door = script.doors[i];

                    if (door.Contain(mouseDownIndex))
                    {
                        script.doors.Remove(door);
                        break;
                    }

                }

            }
        }
    }

    Vector3Int GetGridIndex(Vector3 position)
    {
        Grid grid = script.GetComponentInChildren<Grid>();
        var ray = HandleUtility.GUIPointToWorldRay(position);
        var plane = new Plane(grid.CellToLocal(new Vector3Int(0, 0, 1)), Vector3.zero);
        Vector3Int tilePosition;

        // Compute ray cast with the plane so that this works also in 3D view
        if (plane.Raycast(ray, out var enter))
        {
            var rayPoint = ray.GetPoint(enter);
            tilePosition = grid.WorldToCell(rayPoint);
        }
        // Fallback to the old behaviour just in case
        else
        {
            var mousePosition = ray.origin;
            mousePosition.z = 0;
            tilePosition = grid.WorldToCell(mousePosition);
        }

        tilePosition.z = 0;
        return tilePosition;
    }
}
