using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigLookModule : CameraRigModule
{
    [SerializeField] float sensitivityX = 10;
    [SerializeField] float sensitivityY = 5;
    [SerializeField] bool flipX = false;
    [SerializeField] bool flipY = false;
    [SerializeField] float clampOffsetTop = 5;
    [SerializeField] float clampOffsetBot = 25;
    
    float camPitchY = 0;


    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        camPitchY += -mouseInput.y * sensitivityY * Time.smoothDeltaTime * (flipY ? -1 : 1);
        camPitchY = Mathf.Clamp(camPitchY , -90 + clampOffsetBot, 90 - clampOffsetTop);

        transform.Rotate(new Vector3(0, mouseInput.x * sensitivityX * Time.smoothDeltaTime * (flipX ? -1 : 1), 0), Space.World);
        transform.eulerAngles = new Vector3(camPitchY, transform.eulerAngles.y, transform.eulerAngles.z);

    }
}
