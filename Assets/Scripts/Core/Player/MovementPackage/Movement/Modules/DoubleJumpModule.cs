using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpModule : MotorModule
{
    bool canDoubleJump = false; 
    [SerializeField] float jumpHeight = 30f;

    public override void OnUpdate(MotorState state)
    {
        if (state.IsGrounded()) return;

        if (Input.GetKeyDown(KeyCode.Space) && canDoubleJump)
        {
            canDoubleJump = false;
            Jump();
        }
    }

    public void Jump()
    {
        motor.ResetYVel();
        motor.AddVelocity(new Vector3(0, Mathf.Sqrt(jumpHeight), 0));
    }

    public override void ModuleEnable(MotorState state)
    {
        if (state.IsGrounded())
            canDoubleJump = true;
    }
}
