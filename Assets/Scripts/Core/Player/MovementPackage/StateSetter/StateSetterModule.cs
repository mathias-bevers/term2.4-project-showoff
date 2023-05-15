using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class StateSetterModule : MonoBehaviour
{
    private Motor _motor;

    public Motor motor { get => _motor; }

    private void Awake()
    {
        _motor = GetComponent<Motor>();
        OnAwake();
    }

    private void OnEnable()
    {
        _motor?.Register<StateSetterModule>(this);
        OnEnabled();
    }

    private void OnDisable()
    {
        _motor?.Deregister<StateSetterModule>(this);
        OnDisabled();
    }

    public void DoUpdate()
    {
        OnUpdate();
    }

    internal virtual void OnEnabled() { }
    internal virtual void OnDisabled() { }
    internal virtual void OnAwake() { }
    internal virtual void OnUpdate() { }

    internal void SetState(MotorState state)
    {
        motor?.OverrideState(state);
    }
}
