using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monopoly.Protobuf;

public class TilemapDict<T> where T : MonoBehaviour
{
    private Dictionary<string, T> cache = new Dictionary<string, T>();

    public bool TryGetValue(Coordinate coor, out T value)
    {
        return cache.TryGetValue(coor.ToKey(), out value);
    }

    public void Add(Coordinate coor, T value)
    {
        cache.Add(coor.ToKey(), value);
    }

}
