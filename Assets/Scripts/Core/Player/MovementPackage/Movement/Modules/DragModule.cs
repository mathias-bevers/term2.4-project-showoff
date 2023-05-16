using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragModule : MotorModule
{
    [SerializeField] Vector3 groundDrag;
    [SerializeField] Vector3 airDrag;


    public override void OnLateUpdate(MotorState state)
    {
        if (state.IsGrounded())     motor.AddDrag(groundDrag);
        else if (state.IsInAir())   motor.AddDrag(airDrag);
    }
}
