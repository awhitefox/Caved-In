using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private Dictionary<Vector2Int, TileType> dict;

    public int Top { get; private set; }
    public int Right { get; private set; }
    public int Bottom { get; private set; }
    public int Left { get; private set; }

    private void Awake()
    {
        dict = new Dictionary<Vector2Int, TileType>();
    }

    private void OnDrawGizmos()
    {
        if (dict is null)
            return;

        Gizmos.color = Color.white;
        foreach (Vector2Int pos in dict.Keys)
        {
            Gizmos.DrawCube((Vector2)pos, Vector3.one);
        }
    }

    public TileType GetTile(Vector2Int pos) => dict[pos];

    public void SetTile(Vector2Int pos, TileType value)
    {
        dict[pos] = value;
        if (pos.x > Right)
            Right = pos.x;
        if (pos.x < Left)
            Left = pos.y;
        if (pos.y > Top)
            Top = pos.y;
        if (pos.y < Bottom)
            Bottom = pos.y;
    }

    public bool ContainsTileAt(Vector2Int pos) => dict.ContainsKey(pos);

    public void Clear()
    {
        dict.Clear();
        Top = Right = Bottom = Left = 0;
    }
}
