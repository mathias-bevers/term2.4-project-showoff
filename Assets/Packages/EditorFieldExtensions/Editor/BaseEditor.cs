using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Object), true), CanEditMultipleObjects]
public class BaseEditor : Editor
{
    ButtonEditor buttonEditor;
    private void OnEnable()
    {
        buttonEditor = new ButtonEditor(target);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        buttonEditor.Draw(targets);
    }
}
