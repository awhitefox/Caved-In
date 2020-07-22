using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public bool Generated { get; private set; }
    public int? Seed { get; private set; }
    public GenerationConfig GenerationConfig { get; private set; }
    public Dictionary<Vector2Int, Room> MapData { get; } = new Dictionary<Vector2Int, Room>();

    public event Action WorldGenerated;

    public void Generate(int seed, GenerationConfig generationConfig)
    {
        Seed = seed;
        GenerationConfig = generationConfig;
        
        GenerateMap(new System.Random(seed));
        Generated = true;
        WorldGenerated?.Invoke();

        Debug.Log($"World generated! Rooms: {MapData.Count} Seed: {seed}");
    }

    public void Clear()
    {
        Generated = false;
        Seed = null;
        GenerationConfig = null;
        MapData.Clear();
    }

    private void GenerateMap(System.Random random)
    {
        MapData.Clear();
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(Vector2Int.zero);

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            var possible = new List<Vector2Int>(4) { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            var exits = Directions.None;
            int exitsCount = 0;

            // Check adjacent rooms
            foreach (Vector2Int neighbourPos in pos.GetAdjacent())
            {
                if (MapData.TryGetValue(neighbourPos, out Room neighbour))
                {
                    Vector2Int vector = neighbourPos - pos;
                    possible.Remove(vector);

                    var direction = vector.ToDirection();
                    if (neighbour.Exits.HasFlag(direction.GetOpposite()))
                    {
                        exits |= direction;
                        exitsCount++;
                    }
                }
            }

            // Create new exits
            int exitsCreated = 0;
            while (possible.Count > 0)
            {
                int totalRooms = MapData.Count + queue.Count + 1;

                double chance;
                if (totalRooms == GenerationConfig.maxRooms)
                {
                    chance = 0;
                }
                else if (totalRooms < GenerationConfig.minRooms && exitsCreated == 0)
                {
                    chance = 1;
                }
                else
                {
                    chance = GenerationConfig.GetBaseChance(exitsCount);
                }

                if (random.NextDouble() < chance)
                {
                    Vector2Int vector = random.PopFrom(possible);
                    exits |= vector.ToDirection();
                    exitsCount++;
                    exitsCreated++;

                    Vector2Int absolute = pos + vector;
                    if (!queue.Contains(absolute))
                    {
                        queue.Enqueue(absolute);
                    }
                }
                else
                {
                    break;
                }
            }

            // Pick template and create room
            byte[,] template = PickTemplate(random, (int)exits);
            int rotation = PickTemplateRotation(random, (int)exits);
            var room = new Room(template, rotation, exits);
            MapData.Add(pos, room);
        }
    }

    private byte[,] PickTemplate(System.Random random, int mask)
    {
        switch (mask)
        {
            // One exit
            case 1:
            case 2:
            case 4:
            case 8:
                return random.PickFrom(RoomTemplates.OneExit);

            // Two adjacent exits
            case 3: 
            case 6: 
            case 12:
            case 9:
                return random.PickFrom(RoomTemplates.TwoAdjacentExits);

            // Two opposite exits
            case 5: 
            case 10:
                return random.PickFrom(RoomTemplates.TwoOppositeExits);

            // Three exits
            case 7:
            case 14:
            case 13:
            case 11:
                return random.PickFrom(RoomTemplates.ThreeExits);

            // Four exits
            case 15:
                return random.PickFrom(RoomTemplates.FourExits);

            default: throw new ArgumentOutOfRangeException(nameof(mask));
        }
    }

    private int PickTemplateRotation(System.Random random, int mask)
    {
        switch (mask) 
        {
            // One exit
            case 1: return 0;
            case 2: return 90;
            case 4: return 180;
            case 8: return 270;

            // Two adjacent exits
            case 3: return 0;
            case 6: return 90;
            case 12: return 180;
            case 9: return 270;

            // Two opposite exits
            case 5: return 180 * random.Next(0, 2);
            case 10: return 90 + 180 * random.Next(0, 2);

            // Three exits
            case 7: return 0;
            case 14: return 90;
            case 13: return 180;
            case 11: return 270;

            // Four exits
            case 15: return 90 * random.Next(0, 4);

            default: throw new ArgumentOutOfRangeException(nameof(mask));
        }
    }
}
