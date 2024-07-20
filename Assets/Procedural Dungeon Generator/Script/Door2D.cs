using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Door2D
{
    [HideInInspector]
    public BaseRoom room;

    public BaseRoom connectedRoom;
    public Vector3Int lastRandomIndex;
    public Vector3Int start;
    public Vector3Int end;
    public Direction dir;
    public int clearAmount;
    public bool isX;
    public int length
    {
        get
        {
            return (isX) ? Mathf.Abs(end.x - start.x) : Mathf.Abs(end.y - start.y);
        }
    }

    public int lengthDirection
    {
        get
        {
            int direction = (isX) ? (int)Mathf.Sign(end.x - start.x) : (int)Mathf.Sign(end.y - start.y);
            return direction;
        }
    }

    public void GenerateRandomIndex()
    {
        Vector3Int result = start;

        if (isX)
        {
            result.x = Random.Range(start.x, end.x + 1);
        }
        else
        {
            result.y = Random.Range(start.y, end.y + 1);
        }
        lastRandomIndex = result;
    }


    public Door2D(Vector3Int start_, Vector3Int end_, bool isX_,BaseRoom room)
    {
        isX = isX_;
        start = start_;
        end = end_;
        if(isX)
        {
            bool isStartBigger = start.x > end.x;

            if(isStartBigger)
            {
                Vector3Int temp = start;
                start = end;
                end = temp;
            }

            if(start.y < room.collisionRect.center.y) // 문의 위치가 방의 중심점보다 낮다 => South
            {
                dir = Direction.South;
            }
            else
            {
                dir = Direction.North;
            }
        }
        else
        {
            bool isStartBigger = start.y > end.y;

            if (isStartBigger)
            {
                Vector3Int temp = start;
                start = end;
                end = temp;
            }

            if (start.x < room.collisionRect.center.x ) // 문의 위치가 방의 중심점 보다 왼쪽에 있다 => West
            {
                dir = Direction.West;
            }
            else
            {
                dir = Direction.East;
            }
        }
    }

    public List<Door2D> ConnectRoom(BaseRoom room)
    {
        List<Door2D> candidates = new List<Door2D>(room.doors.Count);

        for(int i =0; i < room.doors.Count; i++)
        {
            var other = room.doors[i];

            if(dir.CanConnect(other.dir))
            {
                candidates.Add(other);
            }
        }
        return candidates;
    }

    public bool CanConnect(BaseRoom room)
    {
        for (int i = 0; i < room.doors.Count; i++)
        {
            var other = room.doors[i];

            if (dir.CanConnect(other.dir))
            {
                return true;
            }
        }
        return false;
    }

    public bool CanConnect(Door2D door)
    {
        return dir.CanConnect(door.dir);
    }

    public Vector3[] GetOutlineVertices(Grid grid)
    {
        Vector3[] vertices = new Vector3[4];
        Vector3 startPos = grid.GetCellCenterWorld(start);
        Vector3 endPos = grid.GetCellCenterWorld(end);
        Vector2 halfCell = grid.cellSize * 0.5f ;

        // visualizor are intercept by tilemap gizmos so narrow it down to 0.8f
        halfCell *= 0.8f;

        if(isX)
        {
            int xDirection = lengthDirection;
            vertices[0] = startPos + new Vector3(-xDirection * halfCell.x, halfCell.y);
            vertices[1] = endPos + new Vector3(xDirection * halfCell.x, halfCell.y);
            vertices[2] = endPos + new Vector3(xDirection * halfCell.x, -halfCell.y);
            vertices[3] = startPos + new Vector3(-xDirection * halfCell.x, -halfCell.y);
        }
        else
        {
            int yDirection = lengthDirection;
            vertices[0] = startPos + new Vector3(halfCell.x, -yDirection * halfCell.y);
            vertices[1] = endPos + new Vector3(halfCell.x, yDirection * halfCell.y);
            vertices[2] = endPos + new Vector3(-halfCell.x, yDirection  * halfCell.y);
            vertices[3] = startPos + new Vector3(-halfCell.x, -yDirection * halfCell.y);
        }
        return vertices;
        
    }


    public bool Contain(Vector3Int index)
    {

        if (isX)
        {
            for (int x = start.x; x <= end.x; x++)
            {
                if (index.x == x && index.y == start.y)
                {
                    return true;
                }
            }
        }
        else
        {
            for (int y = start.y; y <= end.y; y++)
            {

                if (index.y == y && index.x == start.x)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
