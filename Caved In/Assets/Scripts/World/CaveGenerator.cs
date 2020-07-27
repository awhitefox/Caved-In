using System;
using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    [Min(0)]
    public int seed;
    public RandomWalkConfig config;

    public Dictionary<Vector2Int, TileType> Map { get; } = new Dictionary<Vector2Int, TileType>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach (Vector2Int pos in Map.Keys)
        {
            Gizmos.DrawCube((Vector2)pos, Vector3.one);
        }
    }

    public void Generate()
    {
        var random = new System.Random(seed);
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

        Map.Clear();
        Map[Vector2Int.zero] = TileType.Ground;

        Debug.Log($"Starting world generation. Seed: {seed}");
        while (Map.Count + walkers.Count <= config.cellsLimit)
        {
            for (int i = walkers.Count - 1; i >= 0; i--)
            {
                Walker walker = walkers[i];
                walker.MakeStep(random, Map);

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
        Debug.Log($"World generataed. Cells: {Map.Count}");
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

        public void MakeStep(System.Random random, Dictionary<Vector2Int, TileType> map)
        {
            Position = random.PickFrom(GetPossible(map));
            Steps++;
            map[Position] = TileType.Ground;
        }

        public void ResetStepCounter()
        {
            Steps = 0;
        }

        private List<Vector2Int> GetPossible(Dictionary<Vector2Int, TileType> map)
        {
            var possible = new List<Vector2Int>();
            Vector2Int[] adjacent = Position.GetAdjacent();

            for (int i = 0; i < adjacent.Length; i++)
            {
                Vector2Int adj = adjacent[i];
                if (!map.ContainsKey(adj))
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
