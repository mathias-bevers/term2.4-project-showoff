using FPSepController;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FPSepController
{
    [CustomEditor(typeof(Transform), false), CanEditMultipleObjects]
    public class OverrideTransform_Editor : Editor
    {        
        Transform transform = null;
        string editorMsg_noTransform = $"This object's transform has been locked to use default local values. Use a child or parent to move it instead.";

        public override void OnInspectorGUI()
        {
            //Is this Transform coupled to a Player-Rig?
            //If not, draw as normal and ignore the rest of this method.
            //If it is, then reset the values and notify the user.
            transform = target as Transform;
            if(transform.TryGetComponent(out OverrideTransform overrideTransform))
            {
                if (!overrideTransform.drawAnyways)
                {
                    if (overrideTransform.editorChangesValues)
                    {
                        transform.localPosition = Vector3.zero;
                        transform.localEulerAngles = Vector3.zero;
                        transform.localScale = Vector3.one;
                    }
                    if (overrideTransform.drawWarning)
                    {
                        EditorGUILayout.HelpBox(editorMsg_noTransform, MessageType.Info);
                    }
                    return;
                }
            }

            defaultEditor.OnInspectorGUI();
        }


        Editor defaultEditor;
        void OnEnable()
        {
            //When this inspector is created, also create the built-in inspector
            defaultEditor = Editor.CreateEditor(targets, Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
            transform = target as Transform;
        }

        void OnDisable()
        {
            //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
            //Also, make sure to call any required methods like OnDisable
            MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (disableMethod != null)
                disableMethod.Invoke(defaultEditor, null);
            DestroyImmediate(defaultEditor);
        }

    }
}