using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class MotorModule : MonoBehaviour
{
    internal Motor motor;
    [SerializeField] MotorState enabledWhen;
    MotorState lastEnabled = 0;

    [SerializeField] bool useExtendedGroundedRange = false;
    bool _lastUseExtendedGroundedRange = false;

    ExtendedGroundedModule extendedGroundedModule = null;
    bool hasModule = false;

    private void Awake()
    {
        HandleMotor();
    }

    void HandleMotor()
    {
        motor = GetComponent<Motor>();
    }

    private void OnEnable()
    {
        if (motor == null) HandleMotor();
        motor.Register<MotorModule>(this);
    }

    private void OnDisable()
    {
        if (motor == null) HandleMotor();
        motor.Deregister(this);
    }

    public void DoUpdate()
    {
        HandleExtended();
        TickFlag(false);
    }

    public void DoLateUpdate()
    {
        HandleExtended();
        TickFlag(true);
    }



    void TickFlag(bool asLate)
    {
        bool hasFlag = HasFlag();
        MotorState motorState = motor.motorState;
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
        bool hasFlag = enabledWhen.HasFlag(motor.motorState);
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
