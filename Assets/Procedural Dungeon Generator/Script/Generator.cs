using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ProceduralDungeonGeneration
{
    public class Generator : MonoBehaviour
    {
        public BaseRoom[] rooms;
        public int roomAmount;
        public int corridorLength = 0;
        public List<BaseRoom> placedRooms;

        public Tilemap wallTilemap;
        public Tilemap groundTilemap;
        public Tile testTile;
        public BaseRoom a;
        public BaseRoom b;

        /// <summary>
        /// https://gamedev.stackexchange.com/questions/150917/how-to-get-all-tiles-from-a-tilemap
        /// </summary>
        /// 
        public void Test()
        {
            CreateRoom(a);
        }

        void GenerateCorridor(BaseRoom startRoom, BaseRoom endRoom, Door2D startDoor, Door2D endDoor)
        {
            Vector3Int startDoorIndex = Vector3Int.RoundToInt(startRoom.transform.position + startDoor.lastRandomIndex);
            Vector3Int endDoorIndex = Vector3Int.RoundToInt(endRoom.transform.position + endDoor.lastRandomIndex);

            var bluePrint = endRoom.corridorBluePrint;
            // Destroy door-clear tiles
            for (int i = 0; i <= startDoor.clearAmount; i++)
            {
                Vector3Int pos = startDoorIndex - (startDoor.dir.Vectorize() * i);
                wallTilemap.SetTile(pos, null);
            }

            for (int i = 0; i <= endDoor.clearAmount; i++)
            {
                Vector3Int pos = endDoorIndex - (endDoor.dir.Vectorize() * i);
                wallTilemap.SetTile(pos, null);
            }
            Vector3Int startDoorDir = startDoor.dir.Vectorize();
            // Make Corridor
            for (int i = 0; i <= corridorLength; i++)
            {
                var pos = startDoorIndex + startDoorDir * i;
                if (startDoorDir == Vector3Int.left || startDoorDir == Vector3Int.right)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector3Int direction = new Vector3Int(0, y, 0);

                        bluePrint.GetHorizontalTile(direction, out TileTempData wall, out TileTempData ground);
                        SetWallnGroundTile(pos + direction, wall, ground);
                    }
                }
                else if (startDoorDir == Vector3Int.up || startDoorDir == Vector3Int.down)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        Vector3Int direction = new Vector3Int(x, 0, 0);
                        bluePrint.GetVerticalTile(direction, out TileTempData wall, out TileTempData ground);
                        SetWallnGroundTile(pos + direction, wall, ground);
                    }

                }
            }
            // create door
            if (startDoorDir == Vector3Int.left || startDoorDir == Vector3Int.right)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int direction = new Vector3Int(0, y, 0);
                    bluePrint.GetHorizontalTile(-startDoorDir + direction, out var startWall, out var startGround);
                    SetWallnGroundTile(startDoorIndex + direction, startWall, startGround);
                    bluePrint.GetHorizontalTile(startDoorDir + direction, out var endWall, out var endGround);
                    SetWallnGroundTile(endDoorIndex + direction, endWall, endGround);
                }


            }
            else if (startDoorDir == Vector3Int.up || startDoorDir == Vector3Int.down)
            {

                for (int x = -1; x <= 1; x++)
                {
                    Vector3Int direction = new Vector3Int(x, 0, 0);
                    bluePrint.GetVerticalTile(-startDoorDir + direction, out var startWall, out var startGround);
                    SetWallnGroundTile(startDoorIndex + direction, startWall, startGround);
                    bluePrint.GetVerticalTile(startDoorDir + direction, out var endWall, out var endGround);
                    SetWallnGroundTile(endDoorIndex + direction, endWall, endGround);
                }

            }



        }

        void CreateRoom(BaseRoom room)
        {
            var bound = room.wallTilemap.cellBounds;

            room.GetTileDatas(out TileTempData[] wall, out TileTempData[] ground);

            for (int x = 0; x < bound.size.x; x++)
            {
                for (int y = 0; y < bound.size.y; y++)
                {
                    int index = x + y * bound.size.x;

                    Vector3Int pos = new Vector3Int(x, y, 0) + (Vector3Int)room.collisionRect.position;
                    SetWallnGroundTile(pos, wall[index], ground[index]);
                }
            }

        }
        void SetWallTile(Vector3Int index, TileTempData data)
        {
            wallTilemap.SetTile(index, data.tileBase);
            wallTilemap.SetTransformMatrix(index, data.matrix4X4);
        }


        void SetWallnGroundTile(Vector3Int index, TileTempData wall = null, TileTempData ground = null)
        {
            SetTile(wallTilemap, index, wall);
            SetTile(groundTilemap, index, ground);
        }


        void SetTile(Tilemap tilemap, Vector3Int index, TileTempData data)
        {
            tilemap.SetTile(index, data.tileBase);
            tilemap.SetTransformMatrix(index, data.matrix4X4);
        }
        public void Generate()
        {

            placedRooms = new List<BaseRoom>(roomAmount);
            BaseRoom firstRoom = QuickMethod.GetRandom(rooms);
            firstRoom = Instantiate(firstRoom, Vector3.zero, Quaternion.identity, transform);
            CreateRoom(firstRoom);
            placedRooms.Add(firstRoom);

            int count = 1;
            int errorCount = 1;
            while (count < roomAmount)
            {
                if (errorCount >= 100)
                {
                    break;
                }
                var placed = RoomWithAvailableDoor();
                placed.ResetCollisionRect();
                var placedRandomDoor = placed.RandomNonConnectedDoor();
                placedRandomDoor.GenerateRandomIndex();
                BaseRoom connectingRoom = QuickMethod.GetRandom(rooms);
                connectingRoom = Instantiate(connectingRoom, placed.transform.position, Quaternion.identity, transform);

                Vector3 roomDoorPos = placed.transform.position + placedRandomDoor.lastRandomIndex + placedRandomDoor.dir.Vectorize();
                if (connectingRoom.ConnectableDoor(placedRandomDoor, out Door2D connectableDoor))
                {
                    connectableDoor.GenerateRandomIndex();
                    var bottomRight = SetRoomPosition2AnchorBottomRight(placed, connectingRoom, placedRandomDoor);
                    connectingRoom.transform.position += new Vector3(bottomRight.x, bottomRight.y);
                    Vector3 otherDoorPos = connectingRoom.transform.position + connectableDoor.lastRandomIndex;
                    connectingRoom.transform.position += roomDoorPos - otherDoorPos;

                    connectingRoom.transform.position += placedRandomDoor.dir.Vectorize() * corridorLength;
                    connectingRoom.ResetCollisionRect();
                    errorCount++;

                    bool isOverlapping = false;
                    foreach (var room in placedRooms)
                    {
                        if (room.collisionRect.Overlaps(connectingRoom.collisionRect))
                        {
                            connectingRoom.gameObject.SetActive(false);
                            isOverlapping = true;
                            break;
                        }
                    }
                    if (!isOverlapping)
                    {
                        placedRandomDoor.connectedRoom = connectingRoom;
                        connectableDoor.connectedRoom = placed;
                        placedRooms.Add(connectingRoom);
                        CreateRoom(connectingRoom);
                        GenerateCorridor(placed, connectingRoom, placedRandomDoor, connectableDoor);

                        count++;
                    }
                }


            }


        }

        BaseRoom RoomWithAvailableDoor()
        {
            List<BaseRoom> rooms = new List<BaseRoom>();
            for (int i = 0; i < placedRooms.Count; i++)
            {
                if (placedRooms[i].isDoorNotOccupied())
                {
                    rooms.Add(placedRooms[i]);
                }
            }
            return QuickMethod.GetRandom(rooms);
        }




        Vector2Int SetRoomPosition2AnchorBottomRight(BaseRoom placedRoom, BaseRoom connectingRoom, Door2D placedRoomDoor)
        {
            var offset = placedRoom.collisionRect.position - connectingRoom.collisionRect.position;

            if (placedRoomDoor.dir == Direction.East)
            {
                offset.x += placedRoom.collisionRect.width;
            }
            else if (placedRoomDoor.dir == Direction.West)
            {
                offset.x -= connectingRoom.collisionRect.width;
            }
            else if (placedRoomDoor.dir == Direction.North)
            {
                offset.y += placedRoom.collisionRect.height;
            }
            else if (placedRoomDoor.dir == Direction.South)
            {
                offset.y -= connectingRoom.collisionRect.height;
            }
            return offset;
        }


        public bool CanPlaceRoom(BaseRoom otherRoom)
        {
            foreach (var room in rooms)
            {
                if (room.collisionRect.Overlaps(otherRoom.collisionRect))
                {
                    return false;
                }
            }
            return true;
        }

        public void Clear()
        {
            groundTilemap.ClearAllTiles();
            wallTilemap.ClearAllTiles();
        }


    }

}

