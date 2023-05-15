using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EditorButton 
{
    readonly string buttonName;
    readonly MethodInfo method;
    readonly ParameterInfo[] parameters;
    readonly GUIContent buttonContent;
    object[] parametersArray;
    readonly bool activeInEditor;

    public EditorButton(MethodInfo method, Button attributeButton)
    {
        buttonName = attributeButton.s_buttonText;
        if (buttonName == string.Empty) buttonName = method.Name.Replace("&", "");
        this.method = method;
        parameters = method.GetParameters();
        parametersArray = new object[method.GetParameters().Length];
        buttonContent = new GUIContent(buttonName, buttonName);
        activeInEditor = attributeButton.b_activeInEditor;
    }

    public void Draw(UnityEngine.Object[] targets)
    {
        if (!activeInEditor && !Application.isPlaying) EditorGUI.BeginDisabledGroup(true);
        bool hitButton = false;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(buttonContent)) hitButton = true;
        
        for (int f = 0; f < parameters.Length; f++)
            HandleParameter(parameters[f].ParameterType, parametersArray[f], out parametersArray[f]);
        GUILayout.EndHorizontal();

        GUILayout.Space(4);
        EditorGUI.EndDisabledGroup();

        if (!hitButton) return;


        foreach (UnityEngine.Object tar in targets)
            method.Invoke(tar, parametersArray);
    }
    void HandleParameter(Type t, object parIn, out object parOut)
    {
        object parObj = parIn;
        parOut = DrawParamElement(t, parObj);
    }

    Type parType;
    object DrawParamElement(Type t, object par)
    {
        par = TrySetPar(t, par);

        parType = t;
        if (IsType(typeof(int))) par = EditorGUILayout.IntField((int)par);
        else if (IsType(typeof(float))) par = EditorGUILayout.FloatField((float)par);
        else if (IsType(typeof(bool))) par = EditorGUILayout.Toggle((bool)par);
        else if (IsType(typeof(string))) par = EditorGUILayout.TextField((string)par);
        else if (t.IsSubclassOf(typeof(UnityEngine.Object))) par = EditorGUILayout.ObjectField((UnityEngine.Object)par, t, true);
        else EditorGUILayout.HelpBox("Incompatible: " + t.Name, MessageType.None);
        return par;
    }
    object TrySetPar(Type t, object par)
    {
        try
        {
            if (par == null && t.IsValueType) par = Activator.CreateInstance(t);
        }
        catch { }
        return par;
    }

    bool IsType(Type type) => type == parType;
    
}
