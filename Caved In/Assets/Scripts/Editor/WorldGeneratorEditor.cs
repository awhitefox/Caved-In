using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    private WorldGenerator worldGenerator;
    private bool manualGeneration = false;
    private int seed;
    private bool useRandomSeed = true;
    private string[] configs;
    private int selectedConfig = 0;

    private void OnEnable()
    {
        worldGenerator = (WorldGenerator)target;
        seed = worldGenerator.Seed ?? 0;
        configs = GenerationConfigs.GetAvailable();
    }

    public override void OnInspectorGUI()
    {
        // Seed field
        EditorGUI.BeginDisabledGroup(!manualGeneration || useRandomSeed);
        seed = EditorGUILayout.IntField("Seed", seed);
        EditorGUI.EndDisabledGroup();

        // Generate and Reset buttons
        EditorGUILayout.BeginHorizontal();
        manualGeneration = GUILayout.Toggle(manualGeneration, "Generate", "Button");
        if (GUILayout.Button("Reset"))
        {
            worldGenerator.Clear();
            seed = 0;
            SceneView.RepaintAll();
        }
        EditorGUILayout.EndHorizontal();

        // Manual generation section
        if (manualGeneration)
        {
            if (!RoomTemplates.Loaded)
            {
                RoomTemplates.Load();
            }
            useRandomSeed = EditorGUILayout.Toggle("Use Random Seed", useRandomSeed);
            selectedConfig = EditorGUILayout.Popup("Generation Config", selectedConfig, configs);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Proceed"))
            {
                if (useRandomSeed)
                {
                    seed = Random.Range(0, int.MaxValue);
                }
                worldGenerator.Generate(seed, GenerationConfigs.Load(configs[selectedConfig]));
                SceneView.RepaintAll();
            }
            if (GUILayout.Button("Reload templates"))
            {
                RoomTemplates.Load();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    public void OnSceneGUI()
    {
        var roomSize = RoomGrid.CellDimensions;
        var halfSize = roomSize / 2;
        float lineWidht = 5f;

        foreach (Vector2Int key in worldGenerator.MapData.Keys)
        {
            var exits = worldGenerator.MapData[key].Exits;
            var pos = Vector2.Scale(key, roomSize) + halfSize;
            if (exits.HasFlag(Directions.North))
            {
                Handles.DrawAAPolyLine(lineWidht, pos, pos + new Vector2(0, halfSize.y));
            }
            if (exits.HasFlag(Directions.East))
            {
                Handles.DrawAAPolyLine(lineWidht, pos, pos + new Vector2(halfSize.x, 0));
            }
            if (exits.HasFlag(Directions.South))
            {
                Handles.DrawAAPolyLine(lineWidht, pos, pos + new Vector2(0, -halfSize.y));
            }
            if (exits.HasFlag(Directions.West))
            {
                Handles.DrawAAPolyLine(lineWidht, pos, pos + new Vector2(-halfSize.x, 0));
            }
        }
    }
}
