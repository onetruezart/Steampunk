using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtencions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int count = list.Count;
        int last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            int randIndex = Random.Range(i, count);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }
}
