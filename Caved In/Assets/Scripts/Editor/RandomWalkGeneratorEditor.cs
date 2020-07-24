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
        if (GUILayout.Button("Start"))
        {
            gen.StartWalking();
        }
        if (GUILayout.Button("Randomize & Start"))
        {
            gen.seed = UnityEngine.Random.Range(0, int.MaxValue);
            gen.StartWalking();
        }
        if (GUILayout.Button("Stop"))
        {
            gen.StopWalking();
        }
    }
}