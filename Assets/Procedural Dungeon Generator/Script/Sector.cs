using System.Collections;
using System.Collections.Generic;
using Edgar.Unity;
using UnityEngine;

public class Sector : MonoBehaviour
{
    public BaseRoom[] rooms;
    public int roomAmount; 
    List<BaseRoom> placedRooms;


    

    public void Generate()
    {
        placedRooms = new List<BaseRoom>(rooms.Length);

        var randomRoom = GetRandom(rooms);

        BaseRoom room = Instantiate(randomRoom,Vector3.zero,Quaternion.identity);
        for(int i = 1; i < roomAmount; i++)
        {
            
        }
    }

    T GetRandom<T>(IList<T> enumerator)
    {

        int randomIndex = Random.Range(0, enumerator.Count);
        return enumerator[randomIndex];
    }



    bool IsInBoundary(Rect boundary,Rect target)
    {
        bool isBoundBiggerRight = boundary.xMax > target.xMax;
        bool isBoundBiggerLeft = boundary.xMin < target.xMin;
        bool isBoundBiggerTop = boundary.yMax > target.yMax;
        bool isBoundBiggerBottom = boundary.yMin < target.yMin;

        return isBoundBiggerRight && isBoundBiggerLeft && isBoundBiggerTop && isBoundBiggerBottom;
    }


}
