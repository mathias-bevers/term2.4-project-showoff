using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigSmoothFace : CameraRigModule
{
    [SerializeField] Transform forwardHelper;
    [SerializeField] bool sanitize;

    private void Update()
    {
       transform.LookAt(forwardHelper);

    }

}
