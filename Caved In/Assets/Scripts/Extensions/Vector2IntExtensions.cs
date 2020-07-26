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
}
