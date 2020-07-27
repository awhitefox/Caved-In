using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CaveGenerator))]
public class CaveGeneratorEditor : Editor
{
    private CaveGenerator gen;
    private bool manualGeneration;
    private bool useRandomSeed;
    private int seed;
    private string[] configs;
    private int selectedConfig;

    private void OnEnable()
    {
        gen = (CaveGenerator)target;
        useRandomSeed = true;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.IntField("Current Seed", gen.CurrentSeed ?? -1);
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);

        manualGeneration = GUILayout.Toggle(manualGeneration, "Manual Generation", "Button");
        if (!manualGeneration)
            return;

        if (configs == null)
            configs = GenerationConfigs.GetAvailable();

        useRandomSeed = EditorGUILayout.Toggle("Use random seed", useRandomSeed);
        if (!useRandomSeed)
            seed = Math.Abs(EditorGUILayout.IntField("New Seed", seed));

        selectedConfig = EditorGUILayout.Popup("Generation Config", selectedConfig, configs);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Proceed"))
        {
            int newSeed = useRandomSeed ? UnityEngine.Random.Range(0, int.MaxValue) : seed;
            GenerationConfig config = GenerationConfigs.Load(configs[selectedConfig]);
            gen.Generate(newSeed, config);
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Reload configs"))
            configs = null;
        
        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();
    }
}