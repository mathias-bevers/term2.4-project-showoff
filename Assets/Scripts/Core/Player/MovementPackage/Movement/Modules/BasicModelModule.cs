using UnityEngine;

public class BasicModelModule : MotorModule
{
    public override void OnLateUpdate(MotorState state)
    {
        motor.model.rotation = Quaternion.FromToRotation(Vector3.forward, motor.cameraRig.forward);
        Vector3 euler = motor.model.rotation.eulerAngles;
        euler = new Vector3(0, euler.y, 0);
        motor.model.rotation = Quaternion.Euler(euler);
    }
}
