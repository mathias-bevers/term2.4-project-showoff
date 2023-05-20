using UnityEngine;

public class MotorModule : RegistryHelper<MotorModule, Motor>
{
    [SerializeField] MotorState enabledWhen;
    MotorState lastEnabled = 0;

    [SerializeField] bool useExtendedGroundedRange = false;
    bool _lastUseExtendedGroundedRange = false;

    ExtendedGroundedModule extendedGroundedModule = null;
    bool hasModule = false;

    protected sealed override void OnUpdate()
    {
        HandleExtended();
        TickFlag(false);
    }

    protected sealed override void OnLateUpdate()
    {
        HandleExtended();
        TickFlag(true);
    }

    void TickFlag(bool asLate)
    {
        bool hasFlag = HasFlag();
        MotorState motorState = registry.motorState;
        if (lastEnabled != motorState)
        {
            lastEnabled = motorState;
            if (hasFlag) ModuleEnable(motorState);
            else ModuleDisable(motorState);
        }
        if (!hasFlag) return;

        if (asLate) OnLateUpdate(motorState);
        else OnUpdate(motorState);
    }

    bool HasFlag()
    {
        bool hasFlag = enabledWhen.HasFlag(registry.motorState);
        if (hasFlag) return hasFlag;
        if (enabledWhen.HasFlag(MotorState.Grounded) && IsOnExtendedGround()) hasFlag = true;
        return hasFlag;
    }

    void HandleExtended()
    {
        if (useExtendedGroundedRange == _lastUseExtendedGroundedRange) return;

        _lastUseExtendedGroundedRange = useExtendedGroundedRange;
        extendedGroundedModule = GetComponent<ExtendedGroundedModule>();
        hasModule = extendedGroundedModule != null;
    }

    public virtual void OnUpdate(MotorState state) { }
    public virtual void OnLateUpdate(MotorState state) { }
    public virtual void ModuleEnable(MotorState state) { }
    public virtual void ModuleDisable(MotorState state) { }

    bool IsOnExtendedGround()
    {
        if (!hasModule) return false;
        return extendedGroundedModule.isOnExtendedGround;
    }
}
