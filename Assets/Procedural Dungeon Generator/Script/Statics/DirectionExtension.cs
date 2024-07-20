using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public static class DirectionExtension
{
    public static bool CanConnect(this Direction self,Direction other)
    {

        if(self == Direction.North && other == Direction.South)
        {
            return true;
        }
        else if(self == Direction.South && other == Direction.North)
        {
            return true;
        }
        else if(self == Direction.East && other == Direction.West)
        {
            return true;
        }
        else if(self == Direction.West && other == Direction.East)
        {
            return true;
        }
        return false;

    }

    public static Vector3Int Vectorize(this Direction self)
    {
        if(self == Direction.North)
        {
            return Vector3Int.up;
        }
        else if(self == Direction.South)
        {
            return Vector3Int.down;
        }
        else if(self == Direction.East)
        {
            return Vector3Int.right;
        }
        else if(self == Direction.West)
        {
            return Vector3Int.left;
        }
        else
        {
            Debug.LogError("Cant Find Direction! Check out this instance!");
            return Vector3Int.zero;
        }

    }
}

public static class RectIntExtension
{
    public static bool isIntercepting(this RectInt self,RectInt other,bool includeBoundary = false)
    {
        int x1 = Math.Max(self.x, other.x);
        int x2 = Math.Min(self.x + self.width, other.x + other.width);
        int y1 = Math.Max(self.y, other.y);
        int y2 = Math.Min(self.y + self.height, other.y + other.height);


        return includeBoundary ? (x2 >= x1 && y2 >= y1) : (x2 > x1 && y2 > y1);
    }
}

public enum Direction
{
    North, South, East, West
}