using UnityEngine;
using System.Collections;

public class Fps : MonoBehaviour
{
#if DEBUG
    private float count;

    private IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        QualitySettings.vSyncCount = 0;
    }

    private void OnGUI()
    {
        GUIUtility.ScaleAroundPivot(new Vector2(3, 3), Vector2.zero);
        Rect location = new Rect(5, 85, 85, 25);
        string text = $"FPS: {Mathf.Round(count)}";
        Texture black = Texture2D.linearGrayTexture;
        GUI.DrawTexture(location, black, ScaleMode.StretchToFill);
        GUI.color = Color.black;
        GUI.skin.label.fontSize = 18;
        GUI.Label(location, text);

    }
#endif
}