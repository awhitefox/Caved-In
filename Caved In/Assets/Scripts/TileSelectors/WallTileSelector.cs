using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = nameof(WallTileSelector), menuName = "ScriptableObjects/TileSelectors/Wall")]
public class WallTileSelector : TileSelectorBase
{
    public TileBase wallN;
    public TileBase wallE;
    public TileBase wallS;
    public TileBase wallW;
    public TileBase wallCornerInnerNE;
    public TileBase wallCornerInnerNW;
    public TileBase wallCornerInnerSE;
    public TileBase wallCornerInnerSW;
    public TileBase wallCornerOuterNE;
    public TileBase wallCornerOuterNW;
    public TileBase wallCornerOuterSE;
    public TileBase wallCornerOuterSW;
    public TileBase wallCornerOuterDoubleEW;
    public TileBase wallCornerOuterDoubleWE;

    public override TileBase Pick(Vector2Int pos, Map map)
    {
        int mask = 0;
        mask += !map.TryGetTile(pos + Vector2Int.up, out TileType tile) || tile == TileType.Wall ? 1 : 0;
        mask += !map.TryGetTile(pos + Vector2Int.right, out tile) || tile == TileType.Wall ? 2 : 0;
        mask += !map.TryGetTile(pos + Vector2Int.down, out tile) || tile == TileType.Wall ? 4 : 0;
        mask += !map.TryGetTile(pos + Vector2Int.left, out tile) || tile == TileType.Wall ? 8 : 0;
        switch (mask)
        {
            case 11: return wallN;
            case 7: return wallE;
            case 14: return wallS;
            case 13: return wallW;
            case 3: return wallCornerInnerNE;
            case 9: return wallCornerInnerNW;
            case 6: return wallCornerInnerSE;
            case 12: return wallCornerInnerSW;
            case 15: return PickOuterCorner(pos, map);
            default: throw new ArgumentException("Impossible wall position", nameof(map));
        }
    }

    private TileBase PickOuterCorner(Vector2Int pos, Map map)
    {
        int cornerMask = 0;
        cornerMask += map.TryGetTile(new Vector2Int(pos.x - 1, pos.y + 1), out TileType tile) && tile != TileType.Wall ? 1 : 0;
        cornerMask += map.TryGetTile(new Vector2Int(pos.x + 1, pos.y + 1), out tile) && tile != TileType.Wall ? 2 : 0;
        cornerMask += map.TryGetTile(new Vector2Int(pos.x + 1, pos.y - 1), out tile) && tile != TileType.Wall ? 4 : 0;
        cornerMask += map.TryGetTile(new Vector2Int(pos.x - 1, pos.y - 1), out tile) && tile != TileType.Wall ? 8 : 0;
        switch (cornerMask)
        {
            case 1: return wallCornerInnerNW;
            case 2: return wallCornerInnerNE;
            case 4: return wallCornerInnerSE;
            case 8: return wallCornerInnerSW;
            case 5: return wallCornerOuterDoubleWE;
            case 10: return wallCornerOuterDoubleEW;
            default: throw new ArgumentException("Impossible wall position", nameof(map));
        }
    }
}
