using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Vector2))]
public class JoystickCircleEditor : PropertyDrawer
{
    static Texture2D circleTexture;
    static Texture2D pinTexture;

    bool init = false;

    bool hasCircle = false;
    JoystickCircleAttribute attrib;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!init) Init(property);

        if(!hasCircle) DrawOld(position, property, label);
        else
        {
            Rect newPos = position;
            newPos.x += position.width * 0.1f;
            newPos.y += position.height * 0.25f;
            DrawOld(newPos, property, label);
            position.width -= position.width * 0.9f;
            float angle = property.vector2Value.Angle();
            GUI.DrawTexture(position, circleTexture, ScaleMode.ScaleToFit, true, 1);
            Vector2 pivot = new Vector2(position.x + position.width * 0.5f, position.y + position.height * 0.5f);
            GUIUtility.RotateAroundPivot(angle, pivot);

            float mag = property.vector2Value.magnitude;
            if (mag == 0) mag = 0.0001f;
            if (mag > 1) mag = 1;

            GUIUtility.ScaleAroundPivot(new Vector2(mag, mag), pivot);
            GUI.DrawTexture(position, pinTexture, ScaleMode.ScaleToFit, true, 1);
            GUIUtility.ScaleAroundPivot(new Vector2(2 - mag, 2 - mag), pivot);
            GUIUtility.RotateAroundPivot(-angle, pivot);
            
        }
    }

    void Init(SerializedProperty property)
    {
        init = true;
        JoystickCircleAttribute[] attributes = this.fieldInfo.GetCustomAttributes<JoystickCircleAttribute>(false).ToArray();
        if (attributes.Length <= 0) return;

        if (!TryGetCircle())
        {
            attrib = attributes[0];
            hasCircle = true;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (hasCircle) return base.GetPropertyHeight(property, label) * 2;
        return base.GetPropertyHeight(property, label);
    }

    void DrawOld(Rect position, SerializedProperty property, GUIContent label)
    {
        property.vector2Value = EditorGUI.Vector2Field(position, label.text, property.vector2Value);
    }

    bool TryGetCircle()
    {
        circleTexture = Resources.Load<Texture2D>("Images/Pointer");
        pinTexture = Resources.Load<Texture2D>("Images/Pin");
        return circleTexture == null || pinTexture == null;
    }
}
