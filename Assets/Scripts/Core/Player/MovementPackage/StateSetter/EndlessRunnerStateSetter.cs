public class EndlessRunnerStateSetter : StateSetterModule
{
    protected override void OnUpdate()
    {
        if (registry.characterController.isGrounded)
        {
            if (Player.Instance.EffectIsActive(PickupIdentifier.Slippery))
                SetState(MotorState.Slippery);
            else
                SetState(MotorState.Grounded);

        }
        else SetState(MotorState.InAir);
    }
}
