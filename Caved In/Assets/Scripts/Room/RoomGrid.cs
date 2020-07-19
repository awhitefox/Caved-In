using UnityEngine;

public static class RoomGrid
{
    public const int CellSize = 16;
    public static Vector2Int CellDimensions { get; } = new Vector2Int(CellSize, CellSize);

    public static Vector3Int CellToWorldPositon(Vector2Int pos)
    {
        return new Vector3Int(pos.x * CellSize, pos.y * CellSize, 0);
    }

    public static Vector2Int WorldToCellPosition(Vector3 pos)
    {
        int x = (int)pos.x / CellSize;
        int y = (int)pos.y / CellSize;

        x -= pos.x < 0 ? 1 : 0;
        y -= pos.y < 0 ? 1 : 0;

        return new Vector2Int(x, y);
    }

    public static bool IsInCell(int x, int y)
    {
        return 0 <= x && x < CellSize && 0 <= y && y < CellSize;
    }
}
