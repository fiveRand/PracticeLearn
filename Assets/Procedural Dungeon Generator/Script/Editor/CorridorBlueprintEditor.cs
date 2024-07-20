using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CorridorBluePrint))]
public class CorridorBlueprintEditor : Editor
{
    CorridorBluePrint script;

    
    private void OnEnable() {
        script = (CorridorBluePrint)target;

    }



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


    }

    void OnSceneGUI()
    {


        Draw();
    }

    void Draw()
    {

        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }
                Vector3 pos = script.horizontalDoorPoint + new Vector3Int(x, y,0) + Vector3.one * 0.5f;

                if(y == 0)
                {
                    Handles.color = Color.cyan;
                    Handles.DrawWireCube(pos, Vector3.one * 0.9f);
                }
                else
                {

                    Handles.color = Color.white;
                    Handles.DrawWireCube(pos, Vector3.one);
                }
            }
        }

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                Vector3 pos = script.verticalDoorPoint + new Vector3Int(x, y, 0) + Vector3.one * 0.5f;
                if (x == 0)
                {
                    Handles.color = Color.cyan;
                    Handles.DrawWireCube(pos, Vector3.one * 0.9f);
                }
                else
                {

                    Handles.color = Color.white;
                    Handles.DrawWireCube(pos, Vector3.one);
                }
            }
        }

        
    }
}
