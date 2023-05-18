public class BasicStateSetterModule : StateSetterModule
{
    protected override void OnUpdate()
    {
        if (registry.characterController.isGrounded) SetState(MotorState.Grounded);
        else SetState(MotorState.InAir);
    }
}
