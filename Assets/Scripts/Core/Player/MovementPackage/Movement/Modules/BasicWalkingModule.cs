using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JumpModule))]
public class BasicWalkingModule : MotorModule
{
    [SerializeField] float _maxSpeed = 5f;
    [SerializeField] float _maxRunSpeed = 10f;

    [JoystickCircle]
    [SerializeField] 
    Vector2 input;

    public override void OnUpdate(MotorState state)
    {
        CameraRig rig = registry.cameraRig;
        if (rig == null) return;

        Vector3 forw = rig.forward;
        forw.y = 0;
        forw.Normalize();
        Vector3 right = rig.right;
        right.y = 0;
        right.Normalize();


        float speed = _maxSpeed;
        if(Input.GetKey(KeyCode.LeftShift))
            speed = _maxRunSpeed;

        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input.magnitude > 1) input.Normalize();

        Vector2 speedInput = input * speed;

        registry.AddVelocity(forw * speedInput.y);
        registry.AddVelocity(right * speedInput.x);
    }
}
