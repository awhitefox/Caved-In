using System;
using UnityEngine;

public static class Vector2IntExtensions
{
    public static Vector2Int[] GetAdjacent(this Vector2Int vector)
    {
        return new[] {
            vector + Vector2Int.up,
            vector + Vector2Int.right,
            vector + Vector2Int.down,
            vector + Vector2Int.left
        };
    }

    public static Directions ToDirection(this Vector2Int vector)
    {
        if (vector == Vector2.up)
        {
            return Directions.North;
        }
        else if (vector == Vector2Int.right)
        {
            return Directions.East;
        }
        else if (vector == Vector2Int.down)
        {
            return Directions.South;
        }
        else if (vector == Vector2Int.left)
        {
            return Directions.West;
        }
        throw new ArgumentException();
    }
}
