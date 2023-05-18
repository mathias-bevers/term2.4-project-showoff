using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicJumpModule : MotorModule
{

    [SerializeField] float jumpHeight = 30f;
    [SerializeField] bool allowCoyoteTime = false;
    [SerializeField] float coyoteSeconds = 0.4f;

    float timer = 0;

    [SerializeField] KeyCode debugJumpKey;

    //STRICKTLY FORBIDDEN TO USE IT FOR MOVEMENT!!!!!!!!
    //BUT YOU CAN USE IT FOR TIMERS!
    private void Update()
    {
        if (Input.GetKeyDown(debugJumpKey) && allowCoyoteTime)
            timer = coyoteSeconds;
    }

    public override void OnUpdate(MotorState state)
    {
        if (Input.GetKeyDown(debugJumpKey))
            timer = coyoteSeconds;

        if (timer > 0)
            Jump();
        timer -= Time.deltaTime;
    }

    [@Button]
    public void Jump()
    {
        timer = 0;
        registry.ResetYVel();
        registry.AddVelocity(new Vector3(0, Mathf.Sqrt(jumpHeight), 0));
    }
}
