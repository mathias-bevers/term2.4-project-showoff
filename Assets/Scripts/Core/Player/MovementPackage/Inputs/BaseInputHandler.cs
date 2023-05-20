using System;
using UnityEngine;

public class BaseInputHandler : RegistryHelper<BaseInputHandler, Motor> 
{
    protected sealed override void OnEnabled()
    {
        
    }

    protected sealed override void OnDisabled()
    {
        
    }

    protected sealed override void OnUpdate()
    {
        
    }

    protected void RegisterAxis(string internalAxisName, string registeredAxisName, MotorState enabledWhen)
    {

    }
}

public class InputStorage
{
    bool _useStringSystem = true;

    string _internalName;
    KeyCode _internalKey;
    string _registeredName;
    MotorState? _enabledWhen;

    public string internalName { get { return _internalName; } }
    public KeyCode internalKey { get { return _internalKey; } }
    public string registeredName { get { return _registeredName; } }
    public MotorState? enabledWhen { get { return _enabledWhen; } }
    public bool useStringSystem { get { return _useStringSystem; } }

    public InputStorage(string internalName, string registeredName, MotorState? enabledWhen = null)
    {
        _useStringSystem = true;
        _internalName = internalName;
        _registeredName = registeredName;
        _enabledWhen = enabledWhen;
    }
    public InputStorage(KeyCode internalKey, string registeredName, MotorState? enabledWhen = null)
    {
        _useStringSystem = false;
        _internalKey = internalKey;
        _registeredName = registeredName;
        _enabledWhen = enabledWhen;
    }
}

public class AxisStorage : InputStorage
{
    public AxisStorage(string internalAxisName, string registeredAxisName, MotorState? enabledWhen = null) : base(internalAxisName, registeredAxisName, enabledWhen)
    {

    }
}

public class KeyCodeGroup
{

}

public enum KeyState
{
    Up,
    Down,
    Hold
}

[Flags]
public enum InputGroups
{
    group1 = 1,
    group2 = 2,
    group3 = 4,
    group4 = 8,
    group5 = 16,
    group6 = 32,
    group7 = 64,
    group8 = 128,
    group9 = 256,
    group10 = 512,
    group11 = 1024,
    group12 = 2048,
    group13 = 4096,
    group14 = 8192,
    group15 = 16384,
    group16 = 32768,
    group17 = 65536,
    group18 = 131072,
    group19 = 262144,
    group20 = 524288,
    group21 = 1048576,
    group22 = 2097152,
    group23 = 4194304
}
