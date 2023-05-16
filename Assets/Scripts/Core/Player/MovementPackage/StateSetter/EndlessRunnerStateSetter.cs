using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRunnerStateSetter : StateSetterModule
{
    internal override void OnUpdate()
    {
        if (motor.characterController.isGrounded) SetState(MotorState.Grounded);
        else SetState(MotorState.InAir);
    }
}
