using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveAnimationModule : MotorModule
{
    [SerializeField] Animator animator;

    public override void OnUpdate(MotorState state)
    {
        animator?.SetFloat("WalkingSpeed", motor.charVelocity.magnitude);
    }
}
