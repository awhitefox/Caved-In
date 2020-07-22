using System;
using UnityEngine;

[Serializable]
public class GenerationConfig
{
    public int minRooms;
    public int maxRooms;
    [Range(0, 1)] public double secondExitChance;
    [Range(0, 1)] public double thirdExitChance;
    [Range(0, 1)] public double fourthExitChance;

    public double GetBaseChance(int exitsCount)
    {
        switch (exitsCount)
        {
            case 0: return 1;
            case 1: return secondExitChance;
            case 2: return thirdExitChance;
            case 3: return fourthExitChance;
            default: throw new ArgumentOutOfRangeException(nameof(exitsCount));
        }
    }
}
