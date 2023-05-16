using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
[System.Serializable]
public enum MotorState
{
    Grounded = 1,
    InAir = 2,
    Stunned = 4,
    Swimming = 8,
    CustomState0 = 16,
    CustomState1 = 32,
    CustomState2 = 64,
    CustomState3 = 128,
    CustomState4 = 256,
    CustomState5 = 512,
    CustomState6 = 1024,
    CustomState7 = 2048,
    CustomState8 = 4096,
    CustomState9 = 8192,
    CustomState10 = 16384,
    CustomState11 = 32768,
    CustomState12 = 65536,
    CustomState13 = 131072,
    CustomState14 = 262144,
    CustomState15 = 524288,
    CustomState16 = 1048576,
    CustomState17 = 2097152,
    CustomState18 = 4194304
}
