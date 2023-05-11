using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public static class MultiInput
{
   static Keyboard[] keyboardPool;

    [RuntimeInitializeOnLoadMethod]
    static void Setup()
    {
        InputSystem.onAfterUpdate -= OnBeforeUpdate;
        InputSystem.onAfterUpdate += OnBeforeUpdate;
    }

    static int lastCount = 0;

    static void OnBeforeUpdate()
    {
        ReadOnlyArray<InputDevice> inputDevices = InputSystem.devices;
        ReadOnlyArray<InputDevice> disconnectedDevices = InputSystem.disconnectedDevices;

        if (inputDevices.Count == lastCount) return;
        lastCount = inputDevices.Count;
        OnDeviceChange(inputDevices);
    }

    static void OnDeviceChange(ReadOnlyArray<InputDevice> devices)
    {
        List<Keyboard> devicePool = new List<Keyboard>();
        foreach (InputDevice device in devices)
        {
            Debug.Log(device.name);
            if (!device.name.Contains("Keyboard")) continue;
            devicePool.Add(device as Keyboard);
        }

        if (devicePool.Count == 0) return;
        keyboardPool = devicePool.ToArray();
        keyboardPool[0].MakeCurrent();
    }


}
