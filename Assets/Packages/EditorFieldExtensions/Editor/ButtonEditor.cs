using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ButtonEditor
{
    List<EditorButton> buttons = new List<EditorButton>();
    const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    public ButtonEditor(Object target)
    {
        MethodInfo[] methods = target.GetType().GetMethods(flags);
        foreach (MethodInfo method in methods)
        {
            Button attribute = method.GetCustomAttribute<Button>(false);
            if (attribute == null) continue;
            buttons.Add(new EditorButton(method, attribute));
        }
    }

    public void Draw(Object[] targets)
    {
        for(int i = 0; i < buttons.Count; i++)
        {
            EditorButton button = buttons[i];
            button.Draw(targets);
        }
    }
}
