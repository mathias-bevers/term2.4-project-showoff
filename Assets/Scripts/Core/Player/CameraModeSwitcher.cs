using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModeSwitcher : MonoBehaviour
{
    [SerializeField] List<CameraPoints> _cameraPoints = new List<CameraPoints>();
    [SerializeField] CameraFollowPoint followPoint;
    [SerializeField] CameraRigSmoothFace smoothFace;

    int current = 0;

    private void Start()
    {
        Display();
    }
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            current++;
            if (current >= _cameraPoints.Count)
                current = 0;
            Display();
        }
    }*/

    void Display()
    {
        followPoint.followPoint = _cameraPoints[current].cameraPoint;
        smoothFace.forwardHelper = _cameraPoints[current].lookatPoint;
    }
}

[System.Serializable]
public struct CameraPoints
{
    public Transform cameraPoint;
    public Transform lookatPoint;
}
