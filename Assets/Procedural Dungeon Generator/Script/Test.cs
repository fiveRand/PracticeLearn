using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    public RectInt a;
    public RectInt b;

     private void OnDrawGizmos() {
        Gizmos.DrawWireCube(a.center, (Vector2)a.size);
        Gizmos.DrawWireCube(b.center, (Vector2)b.size);

        Debug.Log(a.isIntercepting(b));
    }

    bool IsInBoundary(Rect boundary, Rect target)
    {
        bool isBoundBiggerRight = boundary.xMax > target.xMax;
        bool isBoundBiggerLeft = boundary.xMin < target.xMin;
        bool isBoundBiggerTop = boundary.yMax > target.yMax;
        bool isBoundBiggerBottom = boundary.yMin < target.yMin;

        return isBoundBiggerRight && isBoundBiggerLeft && isBoundBiggerTop && isBoundBiggerBottom;
    }


}
