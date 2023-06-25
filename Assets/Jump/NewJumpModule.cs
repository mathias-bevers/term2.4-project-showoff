using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewJumpModule : MotorModule
{
    [SerializeField] AudioSource jumpSound;
    [SerializeField] AudioSource landSound;

    MotorState lastState = MotorState.CustomState13;

    public override void ModuleEnable(MotorState state)
    {
        if(lastState != state)
        {
            if(lastState == MotorState.InAir)
            {
                landSound?.Play();
            }else if(lastState == MotorState.Grounded)
            {
                jumpSound?.Play();
            }
            lastState = state;
        }
    }
}
