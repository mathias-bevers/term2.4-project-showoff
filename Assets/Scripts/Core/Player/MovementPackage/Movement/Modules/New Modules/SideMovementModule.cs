using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMovementModule : MotorModule
{
    [SerializeField] float maxStickOut = 1.5f;

    public override void OnUpdate(MotorState state)
    {
        HandleInput();
    }


    void HandleInput()
    {

    }
}
