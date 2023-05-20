using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[PreferComponent(typeof(CooldownSharer))]
public class BasicJumpModule : MotorModule
{

    [SerializeField] float jumpHeight = 30f;
    [SerializeField] bool allowCoyoteTime = false;
    [SerializeField] float coyoteSeconds = 0.4f;

    float timer = 0;

    [SerializeField] KeyCode debugJumpKey;

    bool jumpedThisFrame = false;

    CooldownSharer sharer;

    protected override void OnAwake()
    {
        sharer = GetComponent<CooldownSharer>();
    }

    //STRICKTLY FORBIDDEN TO USE IT FOR MOVEMENT!!!!!!!!
    //BUT YOU CAN USE IT FOR TIMERS!
    private void Update()
    {
        timer -= Time.deltaTime;
        if (Input.GetKeyDown(debugJumpKey) && allowCoyoteTime && !jumpedThisFrame)
            timer = coyoteSeconds;
        jumpedThisFrame = false;
    }

    public override void OnUpdate(MotorState state)
    {
        if (Input.GetKeyDown(debugJumpKey))
            timer = coyoteSeconds;
        jumpedThisFrame = false;

        if (timer > 0)
            Jump();

    }

    [@Button]
    public void Jump()
    {
        timer = 0;
        jumpedThisFrame = true;
        registry.ResetYVel();
        registry.AddVelocity(new Vector3(0, Mathf.Sqrt(jumpHeight), 0));
    }
}
