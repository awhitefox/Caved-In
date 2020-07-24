﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkGenerator : MonoBehaviour
{
    [Min(0)]
    public int seed = 0;
    [Range(0, 10)]
    public int[] weights = { 1, 1, 1, 1 };
    public int cells = 8000;
    public bool instant = false;
    [Range(0, 1)]
    public float stepTime = 0.1f;
    public AvoidanceType avoidance;

    private Vector2Int currentPos;
    private System.Random random;
    private bool running = false;

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

    public void StartWalking()
    {
        StopWalking();

        Map.Clear();
        currentPos = Vector2Int.zero;
        Map.Add(currentPos);
        random = new System.Random(seed);

        running = true;
        StartCoroutine(Walk());
    }

    public void StopWalking()
    {
        running = false;
    }

    private IEnumerator Walk()
    {
        while (running && (!instant || Map.Count < cells))
        {
            IList<Vector2Int> possible;
            switch (avoidance)
            {
                case AvoidanceType.None:
                    possible = GetPossibleAny();
                    break;
                case AvoidanceType.Adjacent:
                    possible = GetPossibleAdjacent();
                    if (possible.Count == 0)
                    {
                        goto case AvoidanceType.None;
                    }
                    break;
                case AvoidanceType.Smart:
                    possible = GetPossibleSmart();
                    if (possible.Count == 0)
                    {
                        goto case AvoidanceType.None;
                    }
                    break;
                default:
                    throw new ArgumentException(nameof(avoidance));
            }

            currentPos = random.PickFrom(possible);

            Map.Add(currentPos);
            Debug.Log(Map.Count);
            if (!instant)
            {
                if (stepTime != 0)
                {
                    yield return new WaitForSeconds(stepTime);
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    private List<Vector2Int> GetPossibleAny()
    {
        var possible = new List<Vector2Int>();
        Vector2Int[] adjacent = currentPos.GetAdjacent();
        for (int i = 0; i < adjacent.Length; i++)
        {
            for (int j = 0; j < weights[i]; j++)
            {
                possible.Add(adjacent[i]);
            }
        }
        return possible;
    }
    
    private List<Vector2Int> GetPossibleAdjacent()
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
        return possible;
    }

    private List<Vector2Int> GetPossibleSmart()
    {
        var possible = new List<Vector2Int>();

        Vector2Int[] adjacent = currentPos.GetAdjacent();
        for (int i = 0; i < adjacent.Length; i++)
        {
            Vector2Int adj = adjacent[i];
            if (!Map.Contains(adj))
            {
                bool flag = true;
                foreach (var adj2 in adj.GetAdjacent())
                {
                    if (adj2 != currentPos && Map.Contains(adj2))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    for (int j = 0; j < weights[i]; j++)
                    {
                        possible.Add(adj);
                    }
                }
            }
        }

        return possible;
    }

    public enum AvoidanceType
    {
        None,
        Adjacent,
        Smart
    }
}