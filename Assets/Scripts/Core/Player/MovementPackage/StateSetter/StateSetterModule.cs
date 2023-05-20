public class StateSetterModule : RegistryHelper<StateSetterModule, Motor>
{
    public void SetState(MotorState state)
    {
        registry?.OverrideState(state);
    }
}
