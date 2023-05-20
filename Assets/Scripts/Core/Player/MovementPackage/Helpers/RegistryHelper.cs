using UnityEngine;

public class RegistryHelper<T, TT> : MonoBehaviour where T : RegistryHelper<T, TT> where TT : IRegistry
{
    protected TT _registry;

    public TT registry { get => _registry; }

    private void Awake()
    {
        _registry = GetComponent<TT>();
        OnAwake();
    }

    private void OnEnable()
    {
        _registry?.Register<T>(this as T);
        OnEnabled();
    }

    private void OnDisable()
    {
        _registry?.Deregister<T>(this as T);
        OnDisabled();
    }

    public void DoUpdate()
    {
        OnUpdate();
    }

    public void DoLateUpdate()
    {
        OnLateUpdate();
    }

    protected virtual void OnEnabled() { }
    protected virtual void OnDisabled() { }
    protected virtual void OnAwake() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnLateUpdate() { }
}
