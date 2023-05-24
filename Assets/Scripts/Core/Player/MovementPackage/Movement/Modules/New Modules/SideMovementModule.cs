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
        MoveBackwardsSlightly();
    }


    void MoveForwardSlightly()
    {
        registry.Move(registry.transform.forward * 0.1f);
    }

    void MoveBackwardsSlightly()
    {
        if(motor.transform.localPosition.z >= 0)
        registry.Move(registry.transform.forward * -0.1f);
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            registry.Move(registry.transform.right * -speed);
        if(Input.GetKey(KeyCode.RightArrow))
            registry.Move(registry.transform.right * speed);
    }

    void Clamper()
    {
        Vector3 playerLocalPos = registry.transform.localPosition;
        if (playerLocalPos.x > maxStickOut) playerLocalPos.x = maxStickOut;
        if (playerLocalPos.x < -maxStickOut) playerLocalPos.x = -maxStickOut;
        registry.transform.localPosition = playerLocalPos;
    }
}
