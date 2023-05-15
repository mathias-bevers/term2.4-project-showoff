//Made by: Amber Kortier
using System;
using System.Diagnostics;
using UnityEditor;

/// <summary>
/// A class that runs once at the very beginning that detects when you add a component to an object.
/// </summary>
[InitializeOnLoad]
public class EditorOnComponentAdded
{
    /// <summary>
    /// Runs on runtime. Handles delegate watchers.
    /// </summary>
    static EditorOnComponentAdded()
    {
        ObjectFactory.componentWasAdded -= HandleComponentAdded;
        ObjectFactory.componentWasAdded += HandleComponentAdded;

        EditorApplication.quitting -= OnEditorQuiting;
        EditorApplication.quitting += OnEditorQuiting;
    }

    /// <summary>
    /// Runs when a Unity Component is added via the inspector.
    /// </summary>
    /// <param name="obj">The Component in question that was added.</param>
    private static void HandleComponentAdded(UnityEngine.Component obj)
    {
        HandlePreferComponent(ref obj);
    }

    /// <summary>
    /// Handles the Detection and Intended Execution of the PreferComponent attribute.
    /// </summary>
    /// <param name="obj">A reference to the Component added.</param>
    /// <returns>Returns wether or not this code executed. The code may fail due to the Component itself not being present or preferring a wrongful component.</returns>
    static bool HandlePreferComponent(ref UnityEngine.Component obj)
    {
        PreferComponent[] preferred = (PreferComponent[])Attribute.GetCustomAttributes(obj.GetType(), typeof(PreferComponent));
        if(preferred == null) return false;
        foreach (PreferComponent prefer in preferred)
        {
            if (prefer == null) return false;
            AddComponentForType(prefer.m_Type0, ref obj);
            if (prefer.m_Types1 == null) return true;
            foreach (Type t in prefer.m_Types1)
                AddComponentForType(t, ref obj);
        }
        return true;
    }

    /// <summary>
    /// Adds the specified Type as a component to the given Component.
    /// </summary>
    /// <param name="t">The Type to add as a Component.</param>
    /// <param name="obj">The Unity Object to add the Component to.</param>
    static void AddComponentForType(Type t, ref UnityEngine.Component obj)
    {
        if (t == null) return;
        if (obj.GetComponent(t) != null) return;
        obj.gameObject.AddComponent(t);
    }

    /// <summary>
    /// Runs on editor quit (AKA: When you close the unity editor).
    /// </summary>
    private static void OnEditorQuiting()
    {
        ObjectFactory.componentWasAdded -= HandleComponentAdded;
        EditorApplication.quitting -= OnEditorQuiting;
    }
}