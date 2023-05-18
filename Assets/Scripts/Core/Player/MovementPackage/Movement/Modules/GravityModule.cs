using UnityEngine;

public class GravityModule : MotorModule
{
    [SerializeField] float gravityPower = 1f;
    [SerializeField] bool overrideGravity;
    [SerializeField] Vector3 gravityDirection;

    public override void OnLateUpdate(MotorState state)
    {
        Vector3 gravity = Physics.gravity;
        if (overrideGravity) gravity = gravityDirection;
        registry.AddVelocity(gravity * gravityPower * Time.deltaTime);       
    }

    public override void ModuleEnable(MotorState state)
    {
        if (state.IsInAir()) registry.RemoveFall();
    }
}