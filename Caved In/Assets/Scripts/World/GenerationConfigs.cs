using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class GenerationConfigs
{
    private static readonly string configsPath = Path.Combine(Application.streamingAssetsPath, "Generation");

    public static string[] GetAvailable()
    {
        return Directory.EnumerateFiles(configsPath, "*.json")
                        .Select(s => Path.GetFileNameWithoutExtension(s))
                        .ToArray();
    }

    public static GenerationConfig Load(string configName)
    {
        string path = Path.Combine(configsPath, $"{configName}.json");
        if (!File.Exists(path))
        {
            throw new ArgumentException(nameof(configName));
        }
        return JsonUtility.FromJson<GenerationConfig>(File.ReadAllText(path));
    }
}
