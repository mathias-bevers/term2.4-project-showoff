using UnityEngine;

[PreferComponent(typeof(GravityModule))]
public class SlideModule : MotorModule
{
    [SerializeField] float _slideFriction = 0.5f;
    public override void OnUpdate(MotorState state)
    {
        if (Vector3.Angle(Vector3.up, motor.lastNormal) > motor.characterController.slopeLimit)
        {
            Debug.Log("cringe");
            Vector3 velocity = Vector3.zero;
            velocity.x += (1f - motor.lastNormal.y) * motor.lastNormal.x * (1f - _slideFriction);
            velocity.z += (1f - motor.lastNormal.y) * motor.lastNormal.z * (1f - _slideFriction);


            motor.characterController.Move( velocity);
        }
    }
}
