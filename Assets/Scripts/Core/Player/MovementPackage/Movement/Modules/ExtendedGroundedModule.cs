using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedGroundedModule : MotorModule
{
    [SerializeField] LayerMask collidesWith;

    bool _extendedGround = false;
    public bool isOnExtendedGround { get => _extendedGround; }

    public override void OnUpdate(MotorState state)
    {
        _extendedGround = Physics.SphereCast(transform.position, registry.characterController.radius, Vector3.down, out RaycastHit hit, registry.characterController.stepOffset, collidesWith, QueryTriggerInteraction.Collide);
    }
}
