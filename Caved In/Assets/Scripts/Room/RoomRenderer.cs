using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;

    [Header("Tiles")]
    public TileBase groundTile;
    public TileSelectorBase wallTileSelector;

    public void DrawRoom(Vector2Int gridPos, Room room)
    {
        Vector3Int roomWorldPos = RoomGrid.CellToWorldPositon(gridPos);
        for (int y = 0; y < RoomGrid.CellSize; y++)
        {
            for (int x = 0; x < RoomGrid.CellSize; x++)
            {
                var tilemapPos = GetTilemapPos(roomWorldPos, x, y);

                switch (room.GetTile(x, y))
                {
                    case RoomTile.Ground:
                        groundTilemap.SetTile(tilemapPos, groundTile);
                        break;
                    case RoomTile.Wall:
                        wallTilemap.SetTile(tilemapPos, wallTileSelector.Pick(x, y, room));
                        goto case RoomTile.Ground;
                }
            }
        }
    }

    public void EraseRoom(Vector2Int gridPos)
    {
        var pos = RoomGrid.CellToWorldPositon(gridPos);
        var size = (Vector3Int)RoomGrid.CellDimensions;
        size.z = 1;

        var bounds = new BoundsInt(pos, size);
        var array = new TileBase[256];

        groundTilemap.SetTilesBlock(bounds, array);
        wallTilemap.SetTilesBlock(bounds, array);
    }

    private Vector3Int GetTilemapPos(Vector3Int roomPos, int x, int y)
    {
        return new Vector3Int(roomPos.x + x, roomPos.y + y, 0);
    }
}
