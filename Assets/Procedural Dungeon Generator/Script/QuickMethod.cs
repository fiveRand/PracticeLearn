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
}
