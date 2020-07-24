using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkGenerator : MonoBehaviour
{
    [Min(0)]
    public int seed;
    [Range(0, 10)]
    public int[] defaultWeights;
    public int cellsToShuffle;
    public int cells;
    public int length;
    public LimitBy limitBy;

    private Vector2Int currentPos;
    private System.Random random;
    private Vector2Int minPoint;
    private Vector2Int maxPoint;
    private int shuffles;
    private int[] weights;

    public HashSet<Vector2Int> Map { get; } = new HashSet<Vector2Int>();

    private void OnDrawGizmos()
    {
        if (Map.Count != 0)
        {
            Gizmos.color = Color.white;
            foreach (Vector2Int pos in Map)
            {
                Gizmos.DrawCube((Vector2)pos, Vector3.one);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawCube((Vector2)currentPos, Vector3.one);
        }
    }

    public void Generate()
    {
        Map.Clear();
        currentPos = Vector2Int.zero;
        Map.Add(currentPos);
        random = new System.Random(seed);
        minPoint = Vector2Int.zero;
        maxPoint = Vector2Int.zero;
        shuffles = 0;
        weights = (int[])defaultWeights.Clone();

        Walk();
    }

    private void Walk()
    {
        while (CheckLimit())
        {
            List<Vector2Int> possible = GetPossible();

            currentPos = random.PickFrom(possible);
            minPoint = Vector2Int.Min(minPoint, currentPos);
            maxPoint = Vector2Int.Max(maxPoint, currentPos);

            Map.Add(currentPos);
            int s = Map.Count / cellsToShuffle;
            if (s != shuffles)
            {
                shuffles++;
                ShuffleWeights();
            }
        }
        Debug.Log(Map.Count);
    }

    private void ShuffleWeights()
    {
        var tmp = weights[3];
        weights[3] = weights[1];
        weights[1] = tmp;
    }

    private bool CheckLimit()
    {
        switch (limitBy)
        {
            case LimitBy.Count:
                return Map.Count < cells;
            case LimitBy.Length:
                return (maxPoint - minPoint).sqrMagnitude < length * length;
        }
        return false;
    }
    
    private List<Vector2Int> GetPossible()
    {
        var possible = new List<Vector2Int>();
        Vector2Int[] adjacent = currentPos.GetAdjacent();

        for (int i = 0; i < adjacent.Length; i++)
        {
            Vector2Int adj = adjacent[i];
            if (!Map.Contains(adj))
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

    public enum LimitBy
    {
        Count,
        Length
    }
}
