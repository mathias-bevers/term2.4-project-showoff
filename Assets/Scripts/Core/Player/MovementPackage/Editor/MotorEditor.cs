using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Motor))]
public class MotorEditor : Editor
{
    SerializedProperty motorStateProp;
    SerializedProperty rigProp;
    SerializedProperty modelProp;

    public void OnEnable()
    {
        motorStateProp = GetProperty("_motorState");
        rigProp = GetProperty("_registeredRig");
        modelProp = GetProperty("_model");
    }

    public override void OnInspectorGUI()
    {
        bool before = GUI.enabled;
        GUI.enabled = false;
        EditorGUILayout.HelpBox("The Motor is used to drive movement. NEVER call the Character Controller directly, you may (and most likely should) edit the variables on it however.", MessageType.Info);
        EditorGUILayout.PropertyField(motorStateProp);
        GUI.enabled = before;
        EditorGUILayout.PropertyField(rigProp);
        EditorGUILayout.PropertyField(modelProp);

        serializedObject.ApplyModifiedProperties();
    }


    SerializedProperty GetProperty(string propertyName)
    {
        return serializedObject.FindProperty(propertyName);
    }
}
