using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyModule : MotorModule
{
    [SerializeField] float speed = 3.5f;
    [SerializeField] float accSpeed = 0.2f;
    [SerializeField] float fallbackAggression = 2;
    [SerializeField] float maxStickOut = 1.5f;

    float sidePusher = 0;

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
        if (motor.transform.localPosition.z >= 0)
            registry.Move(registry.transform.forward * -0.1f);
    }

    void HandleInput()
    {
        bool hitKey = false;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            hitKey = true;
            sidePusher += -speed * accSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            hitKey = true;
        sidePusher += speed * accSpeed * Time.deltaTime; }

        if (sidePusher > speed) sidePusher = speed;
        if(sidePusher < -speed) sidePusher = -speed;

        if (!hitKey)
            if (sidePusher > 0) sidePusher -= accSpeed * fallbackAggression * Time.deltaTime;
            else if (sidePusher < 0) sidePusher += accSpeed * fallbackAggression * Time.deltaTime;

        registry.Move(registry.transform.right * sidePusher);
    }

    void Clamper()
    {
        Vector3 playerLocalPos = registry.transform.localPosition;
        if (playerLocalPos.x > maxStickOut) playerLocalPos.x = maxStickOut;
        if (playerLocalPos.x < -maxStickOut) playerLocalPos.x = -maxStickOut;
        registry.transform.localPosition = playerLocalPos;
    }
}