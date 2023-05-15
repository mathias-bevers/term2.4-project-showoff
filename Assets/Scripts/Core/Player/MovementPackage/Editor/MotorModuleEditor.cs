using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MotorModule), true)]
[CanEditMultipleObjects]
public class MotorModuleEditor : Editor
{
    SerializedProperty enabledWhenProp;
    SerializedProperty useExtendedGroundedRangeProp;
    SerializedProperty invertedExtendedProp;

    private void OnEnable()
    {
        enabledWhenProp = serializedObject.FindProperty("enabledWhen");
        useExtendedGroundedRangeProp = serializedObject.FindProperty("useExtendedGroundedRange");
        invertedExtendedProp = serializedObject.FindProperty("invertedExtended");

    }

    public override void OnInspectorGUI()
    {
        bool last = GUI.enabled;
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((MotorModule)target), typeof(MotorModule), false);
        GUI.enabled = last;
        GUILayout.Space(10);    

        EditorGUILayout.PropertyField(enabledWhenProp);
        GUILayout.BeginHorizontal();
        if (((MotorState)enabledWhenProp.enumValueFlag).HasFlag(MotorState.Grounded))
        {
            EditorGUILayout.PropertyField(useExtendedGroundedRangeProp, new GUIContent("Extended Ground: "));
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        DrawPropertiesExcluding(serializedObject, "m_Script","enabledWhen", "useExtendedGroundedRange", "invertedExtended");
        serializedObject.ApplyModifiedProperties();
    }
}
