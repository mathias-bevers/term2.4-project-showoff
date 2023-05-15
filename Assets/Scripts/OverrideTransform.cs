using UnityEngine;

public class OverrideTransform : MonoBehaviour {

    internal bool _drawWarning = true;
    internal bool _editorChangesValues = true;
    internal bool _drawAnyways = false;

    public bool drawWarning { get { return _drawWarning; } }
    public bool editorChangesValues { get { return _editorChangesValues; } }
    public bool drawAnyways { get { return _drawAnyways; } }
}
