using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomWalkGenerator))]
public class RandomWalkGeneratorEditor : Editor
{
    private RandomWalkGenerator gen;

    private void OnEnable()
    {
        gen = (RandomWalkGenerator)target;
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