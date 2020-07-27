using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CaveGenerator))]
public class CaveGeneratorEditor : Editor
{
    private CaveGenerator gen;

    private void OnEnable()
    {
        gen = (CaveGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            gen.Generate();
        }
        if (GUILayout.Button("Randomize & Generate"))
        {
            gen.seed = UnityEngine.Random.Range(0, int.MaxValue);
            gen.Generate();
        }
    }
}