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

    // TODO Make this work with new map
    //public override TileBase Pick(int x, int y, Room room)
    //{
    //    int mask = 0;
    //    mask += room.GetTile(x, y + 1) == RoomTile.Wall ? 1 : 0;
    //    mask += room.GetTile(x + 1, y) == RoomTile.Wall ? 2 : 0;
    //    mask += room.GetTile(x, y - 1) == RoomTile.Wall ? 4 : 0;
    //    mask += room.GetTile(x - 1, y) == RoomTile.Wall ? 8 : 0;

    //    switch (mask)
    //    {
    //        // Walls on edge
    //        case 1: goto case 5;
    //        case 2: goto case 10;
    //        case 4: goto case 5;
    //        case 8: goto case 10;

    //        // Walls
    //        case 5: return room.GetTile(x - 1, y) == RoomTile.Void ? wallW : wallE;
    //        case 10: return room.GetTile(x, y + 1) == RoomTile.Void ? wallN : wallS;

    //        // Corners
    //        case 3: return room.GetTile(x, y - 1) == RoomTile.Void ? wallCornerOuterSW : wallCornerInnerNE;
    //        case 6: return room.GetTile(x, y + 1) == RoomTile.Void ? wallCornerOuterNW : wallCornerInnerSE;
    //        case 12: return room.GetTile(x, y + 1) == RoomTile.Void ? wallCornerOuterNE : wallCornerInnerSW;
    //        case 9: return room.GetTile(x, y - 1) == RoomTile.Void ? wallCornerOuterSE : wallCornerInnerNW;

    //        default: Debug.LogWarning(mask); throw new ArgumentException("Impossible wall position", nameof(room));
    //    }
    //}
}
