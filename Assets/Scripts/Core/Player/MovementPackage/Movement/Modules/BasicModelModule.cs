using UnityEngine;

public class BasicModelModule : MotorModule
{
    public override void OnLateUpdate(MotorState state)
    {
        registry.model.rotation = Quaternion.FromToRotation(Vector3.forward, registry.cameraRig.forward);
        Vector3 euler = registry.model.rotation.eulerAngles;
        euler = new Vector3(0, euler.y, 0);
        registry.model.rotation = Quaternion.Euler(euler);
    }
}
