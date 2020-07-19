using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class RoomTemplates
{
    private static readonly string templatesPath = Path.Combine(Application.streamingAssetsPath, "Rooms");

    public static bool Loaded { get; private set; } = false;
    public static List<byte[,]> OneExit { get; private set; }
    public static List<byte[,]> TwoAdjacentExits { get; private set; }
    public static List<byte[,]> TwoOppositeExits { get; private set; }
    public static List<byte[,]> ThreeExits { get; private set; }
    public static List<byte[,]> FourExits { get; private set; }

    public static void Load()
    {
        OneExit = LoadTemplates("1-*.bin");
        TwoAdjacentExits = LoadTemplates("2a-*.bin");
        TwoOppositeExits = LoadTemplates("2b-*.bin");
        ThreeExits = LoadTemplates("3-*.bin");
        FourExits = LoadTemplates("4-*.bin");
        Loaded = true;

        int i = OneExit.Count + TwoAdjacentExits.Count + TwoOppositeExits.Count + ThreeExits.Count + FourExits.Count;
        Debug.Log($"Loaded {i} room templates");
    }

    private static List<byte[,]> LoadTemplates(string pattern)
    {
        var list = new List<byte[,]>();
        int roomSize = RoomGrid.CellSize;
        foreach (string roomPath in Directory.EnumerateFiles(templatesPath, pattern))
        {
            var room = new byte[roomSize, roomSize];
            using (var reader = new BinaryReader(File.OpenRead(roomPath)))
            {
                for (int y = 0; y < roomSize; y++)
                {
                    for (int x = 0; x < roomSize; x++)
                    {
                        room[x, y] = reader.ReadByte();
                    }
                }
            }
            list.Add(room);
        }
        return list;
    }
}
