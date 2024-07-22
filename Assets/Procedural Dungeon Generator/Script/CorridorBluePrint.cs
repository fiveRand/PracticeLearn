using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProceduralDungeonGeneration
{
    public class CorridorBluePrint : MonoBehaviour
    {
        public Tilemap wallTilemap;
        public Tilemap groundTilemap;

        public Vector3Int horizontalDoorPoint;
        public Vector3Int verticalDoorPoint;


        public TileTempData HorizontalCorridorTile(Vector3Int vec3int)
        {

            var matrix = wallTilemap.GetTransformMatrix(horizontalDoorPoint + vec3int);
            var tile = wallTilemap.GetTile<Tile>(horizontalDoorPoint + vec3int);

            return new TileTempData(tile, matrix);
        }
        public void GetHorizontalTile(Vector3Int index, out TileTempData wallData, out TileTempData groundData)
        {
            var wallMatrix = wallTilemap.GetTransformMatrix(horizontalDoorPoint + index);
            var wallTile = wallTilemap.GetTile<Tile>(horizontalDoorPoint + index);
            wallData = new TileTempData(wallTile, wallMatrix);

            var groundMatrix = groundTilemap.GetTransformMatrix(horizontalDoorPoint + index);
            var groundTile = groundTilemap.GetTile<Tile>(horizontalDoorPoint + index);
            groundData = new TileTempData(groundTile, groundMatrix);
        }


        public TileTempData VerticalCorridorTile(Vector3Int vec3int)
        {
            var matrix = wallTilemap.GetTransformMatrix(verticalDoorPoint + vec3int);
            var tile = wallTilemap.GetTile<Tile>(verticalDoorPoint + vec3int);

            return new TileTempData(tile, matrix);
        }

        public void GetVerticalTile(Vector3Int index, out TileTempData wallData, out TileTempData groundData)
        {
            var wallMatrix = wallTilemap.GetTransformMatrix(verticalDoorPoint + index);
            var wallTile = wallTilemap.GetTile<Tile>(verticalDoorPoint + index);
            wallData = new TileTempData(wallTile, wallMatrix);

            var groundMatrix = groundTilemap.GetTransformMatrix(verticalDoorPoint + index);
            var groundTile = groundTilemap.GetTile<Tile>(verticalDoorPoint + index);
            groundData = new TileTempData(groundTile, groundMatrix);
        }

    }

}
