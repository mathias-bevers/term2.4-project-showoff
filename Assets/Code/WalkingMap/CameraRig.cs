using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField] Camera _camera;
    public Camera Camera { get { return _camera; } }

    private void OnDrawGizmos()
    {
        DrawPoint();
    }

    void DrawPoint()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 0.01f, 0));
    }
}
