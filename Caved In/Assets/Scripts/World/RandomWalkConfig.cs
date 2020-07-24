using System;
using UnityEngine;

[Serializable]
public class RandomWalkConfig
{
    [Min(0)] public int baseWeight;
    [Min(0)] public int increasedWeight;
    [Min(1)] public int stepsToTurn;
}
