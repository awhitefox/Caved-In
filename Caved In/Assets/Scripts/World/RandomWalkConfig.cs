using System;
using UnityEngine;

[Serializable]
public class RandomWalkConfig
{
    [Min(0)] public int cellsLimit;
    [Min(0)] public int baseWeight;
    [Min(0)] public int increasedWeight;
    [Min(1)] public int stepsToBranch;
    [Range(0, 1)] public float branchChance;
}
