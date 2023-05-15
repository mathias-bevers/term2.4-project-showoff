using System;

public class Button : Attribute
{
    public readonly string s_buttonText;
    public readonly bool b_activeInEditor;
    public Button(string buttonText = "", bool activeInEditor = false) { s_buttonText = buttonText; b_activeInEditor = activeInEditor; }
}
