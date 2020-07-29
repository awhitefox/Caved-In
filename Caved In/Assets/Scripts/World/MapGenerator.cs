using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using Stopwatch = System.Diagnostics.Stopwatch;

[RequireComponent(typeof(Map))]
public class MapGenerator : MonoBehaviour
{
    // TODO Move this somewhere else
    private static readonly int[] possibleWallMasks = new[] { 3, 6, 9, 12, 7, 11, 13, 14 };
    private static readonly int[] smoothingMasks = new[] { 7, 28, 112, 193 };

    private Map map;

    public int? CurrentSeed { get; private set; }
    public GenerationConfig CurrentConfig { get; private set; }

    private void Awake()
    {
        map = GetComponent<Map>();
    }

    private void Start()
    {
        Generate(Environment.TickCount, GenerationConfigs.Load("default")); // FIXME
    }

    public void Generate(int seed, GenerationConfig config)
    {
        if (seed < 0)
            throw new ArgumentOutOfRangeException(nameof(seed));
        Debug.Log($"Starting world generation. Seed: {seed}");

        var random = new Random(seed);

        PerformRandomWalk(random, config);
        FixTiles(config);
        SmoothTiles();
        PlaceWalls();

        CurrentSeed = seed;
        CurrentConfig = config;
    }

    private void PerformRandomWalk(Random random, GenerationConfig config)
    {
        var sw = Stopwatch.StartNew();
        int mainDirection = random.Next(4);

        int[] baseWeights = new int[4];
        int[] altWeights = new int[4];
        for (int i = 0; i < 4; i++)
        {
            int value;
            if (i == mainDirection)
            {
                value = config.increasedWeight;
            }
            else
            {
                value = config.baseWeight;
            }
            baseWeights[i] = value;
            altWeights[i] = value;
        }
        baseWeights[(mainDirection + 5) % 4] = config.increasedWeight;
        altWeights[(mainDirection + 3) % 4] = config.increasedWeight;

        var walkers = new List<Walker>()
        {
            new Walker(Vector2Int.zero, random.PickFromParams(baseWeights, altWeights))
        };

        map.Clear();
        map.SetTile(Vector2Int.zero, TileType.Ground);

        while (map.TileCount + walkers.Count <= config.cellsLimit)
        {
            for (int i = walkers.Count - 1; i >= 0; i--)
            {
                Walker walker = walkers[i];
                walker.MakeStep(random, map);

                if (walker.Steps == config.stepsToBranch)
                {
                    if (i == 0)
                    {
                        walker.ResetStepCounter();
                        walker.Weights = random.PickFromParams(baseWeights, altWeights);

                        if (random.NextDouble() < config.branchChance)
                        {
                            var weights = walker.Weights == baseWeights ? altWeights : baseWeights;
                            walkers.Add(new Walker(walker.Position, weights));
                        }
                    }
                    else
                    {
                        walkers.RemoveAt(i);
                    }
                }
            }
        }
        sw.Stop();
        Debug.Log($"Random walk completed in {sw.ElapsedMilliseconds} ms, {map.TileCount} tiles placed.");
    }

    private void FixTiles(GenerationConfig config)
    {
        var sw = Stopwatch.StartNew();
        var tilesToFix = GetTilesToFix();

        int i = 0;
        while (tilesToFix.Count > 0)
        {
            Vector2Int pos = tilesToFix.Dequeue();
            map.SetTile(pos, TileType.Ground);
            i++;
            foreach (var adj in pos.GetAdjacent())
            {
                if (!map.ContainsTileAt(adj) && CheckIfTileNeedsFix(adj) && !tilesToFix.Contains(adj))
                {
                    tilesToFix.Enqueue(adj);
                }
            }
            if (i > config.cellsLimit)
            {
                Debug.LogWarning("Something went wrong during tile fixing, aborting.");
                return;
            }
        }
        sw.Stop();
        Debug.Log($"Tile fixing completed in {sw.ElapsedMilliseconds} ms, {i} tiles placed.");
    }

    private void SmoothTiles()
    {
        var tilesToRemove = new HashSet<Vector2Int>();
        foreach (Vector2Int pos in map)
        {
            int mask = 0;
            mask += map.ContainsTileAt(new Vector2Int(pos.x, pos.y + 1)) ? 1 : 0;
            mask += map.ContainsTileAt(new Vector2Int(pos.x + 1, pos.y + 1)) ? 2 : 0;
            mask += map.ContainsTileAt(new Vector2Int(pos.x + 1, pos.y)) ? 4 : 0;
            mask += map.ContainsTileAt(new Vector2Int(pos.x + 1, pos.y - 1)) ? 8 : 0;
            mask += map.ContainsTileAt(new Vector2Int(pos.x, pos.y - 1)) ? 16 : 0;
            mask += map.ContainsTileAt(new Vector2Int(pos.x - 1, pos.y - 1)) ? 32 : 0;
            mask += map.ContainsTileAt(new Vector2Int(pos.x - 1, pos.y)) ? 64 : 0;
            mask += map.ContainsTileAt(new Vector2Int(pos.x - 1, pos.y + 1)) ? 128 : 0;
            if (smoothingMasks.Contains(mask))
            {
                tilesToRemove.Add(pos);
            }
        }
        foreach (Vector2Int pos in tilesToRemove)
        {
            map.RemoveTileAt(pos);
        }
    }

    private void PlaceWalls()
    {
        var tilesToAdd = new HashSet<Vector2Int>();
        foreach (Vector2Int pos in map)
        {
            for (int y = pos.y - 1; y <= pos.y + 1; y++)
            {
                for (int x = pos.x - 1; x <= pos.x + 1; x++)
                {
                    var neighbourPos = new Vector2Int(x, y);
                    if (!map.ContainsTileAt(neighbourPos))
                    {
                        tilesToAdd.Add(neighbourPos);
                    }
                }
            }
        }
        foreach (Vector2Int pos in tilesToAdd)
        {
            map.SetTile(pos, TileType.Wall);
        }
    }

    private Queue<Vector2Int> GetTilesToFix()
    {
        var tilesToFix = new Queue<Vector2Int>();
        var scanQueue = new Queue<Vector2Int>();
        var scannedTiles = new HashSet<Vector2Int>();

        scanQueue.Enqueue(Vector2Int.zero);
        scannedTiles.Add(Vector2Int.zero);

        while (scanQueue.Count > 0)
        {
            var pos = scanQueue.Dequeue();
            foreach (var adj in pos.GetAdjacent())
            {
                if (scannedTiles.Contains(adj))
                    continue;
                scannedTiles.Add(adj);

                if (map.ContainsTileAt(adj))
                {
                    scanQueue.Enqueue(adj);
                }
                else if (CheckIfTileNeedsFix(adj))
                {
                    tilesToFix.Enqueue(adj);
                }
            }
        }
        return tilesToFix;
    }

    private bool CheckIfTileNeedsFix(Vector2Int pos)
    {
        int mask = 0;
        mask += !map.ContainsTileAt(pos + Vector2Int.up) ? 1 : 0;
        mask += !map.ContainsTileAt(pos + Vector2Int.right) ? 2 : 0;
        mask += !map.ContainsTileAt(pos + Vector2Int.down) ? 4 : 0;
        mask += !map.ContainsTileAt(pos + Vector2Int.left) ? 8 : 0;
        return !possibleWallMasks.Contains(mask);
    }

    private class Walker
    {
        private int[] weights;

        public Vector2Int Position { get; private set; }
        public int Steps { get; private set; }
        public int[] Weights
        {
            get => weights;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length != 4)
                    throw new ArgumentException(nameof(value));
                weights = value;
            }
        }

        public Walker(Vector2Int pos, int[] weights)
        {
            Position = pos;
            Steps = 0;
            Weights = weights;
        }

        public void MakeStep(Random random, Map map)
        {
            Position = random.PickFrom(GetPossible(map));
            Steps++;
            map.SetTile(Position, TileType.Ground);
        }

        public void ResetStepCounter()
        {
            Steps = 0;
        }

        private List<Vector2Int> GetPossible(Map map)
        {
            var possible = new List<Vector2Int>();
            Vector2Int[] adjacent = Position.GetAdjacent();

            for (int i = 0; i < adjacent.Length; i++)
            {
                Vector2Int adj = adjacent[i];
                if (!map.ContainsTileAt(adj))
                {
                    for (int j = 0; j < weights[i]; j++)
                    {
                        possible.Add(adj);
                    }
                }
            }

            if (possible.Count == 0)
            {
                for (int i = 0; i < adjacent.Length; i++)
                {
                    for (int j = 0; j < weights[i]; j++)
                    {
                        possible.Add(adjacent[i]);
                    }
                }
            }

            return possible;
        }
    }
}
