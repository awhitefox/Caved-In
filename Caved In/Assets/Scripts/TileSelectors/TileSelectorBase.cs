using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class TileSelectorBase : ScriptableObject
{
    public abstract TileBase Pick(int x, int y, Room room);
}
