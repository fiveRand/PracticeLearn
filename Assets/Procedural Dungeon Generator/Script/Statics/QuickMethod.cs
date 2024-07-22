using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuickMethod 
{
    public static T GetRandom<T>(IList<T> enumerator)
    {

        int randomIndex = Random.Range(0, enumerator.Count);
        return enumerator[randomIndex];
    }

    public static Vector2 RandomDirection()
    {
        float xAng = Random.Range(0, 360);
        float yAng = Random.Range(0, 360);
        return new Vector2(Mathf.Cos(xAng), Mathf.Sin(yAng));
    }

    public static Vector3[] RectangleVertices(Vector3Int bottomLeft, Vector3Int topRight)
    {

        Vector3[] vertices = GetOutlineVertices(bottomLeft, topRight);
        return vertices;
    }

    static Vector3[] GetOutlineVertices(Vector3Int bottomLeft, Vector3Int topRight)
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = bottomLeft;
        vertices[1] = new Vector3(bottomLeft.x, topRight.y);
        vertices[2] = topRight;
        vertices[3] = new Vector3(topRight.x, bottomLeft.y);
        return vertices;

    }
}
