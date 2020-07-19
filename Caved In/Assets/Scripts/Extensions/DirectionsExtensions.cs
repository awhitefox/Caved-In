using System;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionsExtensions
{
    public static Directions GetOpposite(this Directions directions)
    {
        switch (directions)
        {
            case Directions.North: return Directions.South;
            case Directions.East: return Directions.West;
            case Directions.South: return Directions.North;
            case Directions.West: return Directions.East;
            default: throw new ArgumentException();
        }
    }

    public static List<Vector2Int> ToVectorList(this Directions directions)
    {
        var list = new List<Vector2Int>(4);
        if (directions.HasFlag(Directions.North))
        {
            list.Add(Vector2Int.up);
        }
        if (directions.HasFlag(Directions.East))
        {
            list.Add(Vector2Int.right);
        }
        if (directions.HasFlag(Directions.South))
        {
            list.Add(Vector2Int.down);
        }
        if (directions.HasFlag(Directions.West))
        {
            list.Add(Vector2Int.left);
        }
        return list;
    }
}