using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMovementModule : MotorModule
{
    [SerializeField] float speed = 3.5f;
    [SerializeField] float maxStickOut = 1.5f;

    public override void OnUpdate(MotorState state)
    {
        HandleInput();
        Clamper();
        MoveForwardSlightly();
    }

    public override void OnLateUpdate(MotorState state)
    {
        MoveBackwardsSlightly();
    }

    void MoveForwardSlightly()
    {
        motor.Move(motor.transform.forward * 0.1f);
    }

    void MoveBackwardsSlightly()
    {
        motor.Move(motor.transform.forward * -0.1f);
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            motor.Move(motor.transform.right * -speed);
        if(Input.GetKey(KeyCode.RightArrow))
            motor.Move(motor.transform.right * speed);
    }

    void Clamper()
    {
        Vector3 playerLocalPos = motor.transform.localPosition;
        if (playerLocalPos.x > maxStickOut) playerLocalPos.x = maxStickOut;
        if (playerLocalPos.x < -maxStickOut) playerLocalPos.x = -maxStickOut;
        motor.transform.localPosition = playerLocalPos;
    }
}
