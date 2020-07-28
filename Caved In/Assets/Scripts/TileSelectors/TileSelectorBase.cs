using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class TileSelectorBase : ScriptableObject
{
    public abstract TileBase Pick(Vector2Int pos, Map map);
}
