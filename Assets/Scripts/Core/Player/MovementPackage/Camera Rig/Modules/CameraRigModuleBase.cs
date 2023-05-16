using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigModuleBase : OverrideTransform
{
    public CameraRigModuleBase()
    {
        _drawWarning = false;
        _editorChangesValues = false;
        _drawAnyways = true;
    }

}
