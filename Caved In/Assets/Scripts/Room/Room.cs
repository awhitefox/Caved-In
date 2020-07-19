public class Room
{
    private readonly byte[,] template;
    private readonly int rotation;

    public Directions Exits { get; }

    public Room(byte[,] template, int rotation, Directions exits)
    {
        this.template = template;
        this.rotation = rotation;
        Exits = exits;
    }

    public RoomTile GetTile(int x, int y)
    {
        if (RoomGrid.IsInCell(x, y))
        {
            RotatePos(ref x, ref y);
            return (RoomTile)template[x, y];
        }
        else
        {
            return RoomTile.Void;
        }
    }

    private void RotatePos(ref int x, ref int y)
    {
        int size = RoomGrid.CellSize;
        int i = x + y * size;
        switch (rotation)
        {
            case 0:
                x = i % size;
                y = i / size;
                break;
            case 90:
                x = size - i / size - 1;
                y = i % size;
                break;
            case 180:
                x = size - i % size - 1;
                y = size - i / size - 1;
                break;
            case 270:
                x = i / size;
                y = size - i % size - 1;
                break;
        }
    }
}
