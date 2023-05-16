using System;

public class ButtonAttribute : Attribute
{
    public readonly string s_buttonText;
    public readonly bool b_activeInEditor;
    public ButtonAttribute(string buttonText = "", bool activeInEditor = false) { s_buttonText = buttonText; b_activeInEditor = activeInEditor; }
}
