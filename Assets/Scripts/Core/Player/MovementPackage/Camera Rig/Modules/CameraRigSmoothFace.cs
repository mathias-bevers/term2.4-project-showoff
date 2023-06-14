using UnityEngine;

public class CameraRigSmoothFace : CameraRigModule
{
   public Transform forwardHelper;
    public float speed = 10;

    private void Update()
    {
        //transform.LookAt(forwardHelper);
        //return;
        Quaternion targetRotation = Quaternion.LookRotation(forwardHelper.position - transform.position);
        Quaternion currentRotation = transform.rotation;
        Vector3 newEueler = new Vector3(currentRotation.eulerAngles.x, targetRotation.eulerAngles.y, currentRotation.eulerAngles.z);
        //transform.LookAt(forwardHelper);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);

        
    }
}
