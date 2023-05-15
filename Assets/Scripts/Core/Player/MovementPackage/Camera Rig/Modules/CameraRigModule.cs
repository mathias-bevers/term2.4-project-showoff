using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRig))]
public class CameraRigModule : CameraRigModuleBase
{
    internal CameraRig cameraRig;

   
    public virtual void Awake()
    {
        cameraRig = GetComponent<CameraRig>();
    }

}
