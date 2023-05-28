using NaughtyAttributes;
using UnityEngine;

public class JumpModule : MotorModule
{

    [SerializeField] float jumpHeight = 30f;

#if UNITY_EDITOR
    [SerializeField] KeyCode debugJumpKey;

    public override void OnUpdate(MotorState state)
    {
        
        if (Input.GetKeyDown(debugJumpKey))
            Jump();
    }
#endif

    [Button]
    public void Jump()
    {
        registry.ResetYVel();
        registry.AddVelocity(new Vector3(0, Mathf.Sqrt(jumpHeight), 0));
    }
}
