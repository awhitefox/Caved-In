using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkGenerator : MonoBehaviour
{
    [Min(0)]
    public int seed;
    public RandomWalkConfig config;

    public int cells;
    public int length;
    public LimitBy limitBy;

    public HashSet<Vector2Int> Map { get; } = new HashSet<Vector2Int>();

    public enum LimitBy
    {
        Count,
        Length
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach (Vector2Int pos in Map)
        {
            Gizmos.DrawCube((Vector2)pos, Vector3.one);
        }
    }

    public void Generate()
    {
        var rnd = new System.Random(seed);
        var minCell = Vector2Int.zero;
        var maxCell = Vector2Int.zero;

        // TODO Randomly generate this
        int[] baseWeights = { 3, 3, 2, 2 };
        int[] altWeights = { 3, 2, 2, 3 };

        var walkers = new List<Walker>()
        {
            new Walker(Vector2Int.zero, baseWeights)
        };

        Map.Clear();
        Map.Add(Vector2Int.zero);

        while (CheckLimit(minCell, maxCell))
        {
            for (int i = walkers.Count - 1; i >= 0; i--)
            {
                Walker walker = walkers[i];
                walker.MakeStep(rnd, Map);

                minCell = Vector2Int.Min(minCell, walker.Position);
                maxCell = Vector2Int.Max(maxCell, walker.Position);

                if (walker.Steps == config.stepsToTurn)
                {
                    switch (rnd.Next(walkers.Count > 1 ? 3 : 2))
                    {
                        case 0:
                            walker.ResetStepCounter();
                            walker.Weights = rnd.PickFromParams(baseWeights, altWeights);
                            break;
                        case 1:
                            walker.ResetStepCounter();
                            walker.Weights = rnd.PickFromParams(baseWeights, altWeights);
                            walkers.Add(new Walker(walker.Position, walker.Weights == baseWeights ? altWeights : baseWeights));
                            break;
                        case 2:
                            walkers.RemoveAt(i);
                            break;
                    }
                }
            }
        }
    }

    private bool CheckLimit(Vector2Int minCell, Vector2Int maxCell)
    {
        switch (limitBy)
        {
            case LimitBy.Count:
                return Map.Count < cells;
            case LimitBy.Length:
                return (maxCell - minCell).sqrMagnitude < length * length;
        }
        return false;
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

        public void MakeStep(System.Random rnd, HashSet<Vector2Int> map)
        {
            Position = rnd.PickFrom(GetPossible(map));
            Steps++;
            map.Add(Position);
        }

        public void ResetStepCounter()
        {
            Steps = 0;
        }

        private List<Vector2Int> GetPossible(HashSet<Vector2Int> map)
        {
            var possible = new List<Vector2Int>();
            Vector2Int[] adjacent = Position.GetAdjacent();

            for (int i = 0; i < adjacent.Length; i++)
            {
                Vector2Int adj = adjacent[i];
                if (!map.Contains(adj))
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
