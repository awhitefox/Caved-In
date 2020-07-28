using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour, IEnumerable<Vector2Int>
{
    private Dictionary<Vector2Int, TileType> dict;

    public int TileCount => dict.Count;

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

    public void SetTile(Vector2Int pos, TileType value) => dict[pos] = value;

    public bool RemoveTileAt(Vector2Int pos) => dict.Remove(pos);

    public bool ContainsTileAt(Vector2Int pos) => dict.ContainsKey(pos);

    public void Clear() => dict.Clear();

    IEnumerator<Vector2Int> IEnumerable<Vector2Int>.GetEnumerator() => dict.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => dict.Keys.GetEnumerator();
}
