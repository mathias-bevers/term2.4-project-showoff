using UnityEngine;

public class CameraRigSmoothFace : CameraRigModule
{
   public Transform forwardHelper;

    private void LateUpdate()
    {
       transform.LookAt(forwardHelper);
    }
}
