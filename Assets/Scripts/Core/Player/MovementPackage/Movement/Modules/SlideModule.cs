using UnityEngine;

[PreferComponent(typeof(GravityModule))]
public class SlideModule : MotorModule
{
    [SerializeField] float _slideFriction = 0.5f;
    public override void OnUpdate(MotorState state)
    {
        if (Vector3.Angle(Vector3.up, registry.lastNormal) > registry.characterController.slopeLimit)
        {
            Debug.Log("cringe");
            Vector3 velocity = Vector3.zero;
            velocity.x += (1f - registry.lastNormal.y) * registry.lastNormal.x * (1f - _slideFriction);
            velocity.z += (1f - registry.lastNormal.y) * registry.lastNormal.z * (1f - _slideFriction);


            registry.characterController.Move( velocity);
        }
    }
}
