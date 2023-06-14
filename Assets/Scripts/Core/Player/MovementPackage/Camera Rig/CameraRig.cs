using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField] Camera _registerOnStart;

    [SerializeField] RotElement _cameraRotation;
    CameraAttachPoint _cameraAttachPoint;

    [HideInInspector] public Vector3 forward => transform.forward;
    [HideInInspector] public Vector3 sanitizedForward { get { Vector3 forw = this.forward; forw.y = 0; forw.Normalize(); return forw; } }
    [HideInInspector] public Vector3 right => transform.right;

    public CameraAttachPoint cameraAttachPoint { get => _cameraAttachPoint; }
    Transform _cameraAttachTransform;
    public Transform cameraAttachPointTransform { get => _cameraAttachTransform; }

    Transform cameraTransform;

    public RotElement cameraRotation { get => _cameraRotation; set { _cameraRotation = value; UpdateCamRotation(); } }
    public Camera cam => _registerOnStart;

    private void Awake()
    {
        
        if(_cameraAttachPoint == null)
        {
            Transform newPoint = new GameObject("CameraAttachPoint").transform;
            _cameraAttachTransform = newPoint;
            newPoint.parent = transform;
            _cameraAttachPoint = newPoint.AddComponent<CameraAttachPoint>();
        }
        if (_registerOnStart != null) RegisterCamera(_registerOnStart);
    }

    public void RegisterCamera(Camera camera)
    {
        cameraTransform = camera.transform;
        cameraTransform.parent = cameraAttachPointTransform;
        cameraAttachPointTransform.transform.localPosition = Vector3.zero;
        cameraTransform.localRotation = Quaternion.identity;
        cameraTransform.localPosition = Vector3.zero;
        cameraTransform.localRotation = Quaternion.identity;
        UpdateCamRotation();
    }

    private void UpdateCamRotation()
    {
        if (cameraTransform == null) return;
        cameraTransform.localRotation = Quaternion.Euler(_cameraRotation.pitch, _cameraRotation.heading, _cameraRotation.yaw);
    }
}
