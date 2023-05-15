using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Utils {

    public static float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    public static bool IsGrounded(this MotorState motorState) => motorState == MotorState.Grounded;
    public static bool IsInAir(this MotorState motorState) => motorState == MotorState.InAir;
    public static bool IsStunned(this MotorState motorState) => motorState == MotorState.Stunned;

#if false
    public static float Angle(this Vector2 vector2)
    {
        if (vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
        }
    }
#endif
}
