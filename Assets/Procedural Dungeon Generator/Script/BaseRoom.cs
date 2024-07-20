using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseRoom : MonoBehaviour
{
    public RectInt collisionRect;
    public Grid grid;
    public Tilemap wallTilemap;
    public Tilemap groundTilemap;
    public CorridorBluePrint corridorBluePrint;
    public List<Door2D> doors = new List<Door2D>();

    public TileTempData WallGetTileTempData(Vector3Int index)
    {
        var matrix = wallTilemap.GetTransformMatrix(index);
        var tile = wallTilemap.GetTile<Tile>(index);
        return new TileTempData(tile, matrix);
    }

    public void GetTileDatas(out TileTempData[] wall,out TileTempData[] ground)
    {
        BoundsInt bound = wallTilemap.cellBounds;
        wall = new TileTempData[bound.size.x * bound.size.y];
        ground = new TileTempData[bound.size.x * bound.size.y];
        for (int x = 0; x < bound.size.x; x++)
        {
            for (int y = 0; y < bound.size.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0) + bound.min;
                var wallMatrix = wallTilemap.GetTransformMatrix(pos);
                var wallTile = wallTilemap.GetTile<Tile>(pos);
                var groundMatrix = groundTilemap.GetTransformMatrix(pos);
                var groundTile = groundTilemap.GetTile<Tile>(pos);

                wall[x + y * bound.size.x] = new TileTempData(wallTile, wallMatrix);
                ground[x + y * bound.size.x] = new TileTempData(groundTile, groundMatrix);
            }
        }
    }

    public void ResetCollisionRect()
    {
        wallTilemap.CompressBounds();
        Bounds bound = wallTilemap.localBounds;
        var min = Vector2Int.RoundToInt(bound.min + transform.position);

        var size = Vector2Int.RoundToInt(bound.size);

        collisionRect = new RectInt(min, size);
    }

    public bool isDoorNotOccupied()
    {
        foreach(var door in doors)
        {
            if(door.connectedRoom == null)
            {
                return true;
            }
        }
        return false;
    }
    public Door2D RandomNonConnectedDoor()
    {
        List<Door2D> candidates = new List<Door2D>(doors.Count);
        foreach(var door in doors)
        {
            if(door.connectedRoom != null)
            {
                continue;
            }
            candidates.Add(door);
        }

        return QuickMethod.GetRandom(candidates);
        
    }

    public bool ConnectableDoor(Door2D otherDoor,out Door2D connectableDoor)
    {
        connectableDoor = null;
        foreach (var door in doors)
        {
            if (door.connectedRoom != null)
            {
                continue;
            }
            if(door.CanConnect(otherDoor))
            {
                connectableDoor = door;
                return true;
            }

        }
        return false;
    }

}
