using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraPushbackModule))]
public class CameraRigScrollModule : CameraRigModule
{
    CameraPushbackModule _pushbackModule;

    [SerializeField] float _scrollSpeed = 10;

    private void Start()
    {
        _pushbackModule= GetComponent<CameraPushbackModule>();
    }


    private void Update()
    {
        _pushbackModule.allowedMaxDistance += (-Input.mouseScrollDelta.y * _scrollSpeed * Time.deltaTime);
    }

}
