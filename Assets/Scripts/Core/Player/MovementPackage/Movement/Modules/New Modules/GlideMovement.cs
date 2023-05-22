using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlideMovement : MotorModule
{
    [SerializeField] float glideTime = 1.0f;
    //[SerializeField] float jumpHeight = 30f;
    [SerializeField] bool allowCoyoteTime = false;
    [SerializeField] float coyoteSeconds = 0.4f;

    [SerializeField] float timer = 0;

    [SerializeField] KeyCode debugJumpKey;

    bool jumpedThisFrame = false;

    CharacterControllerData ccData;
    [SerializeField] float glideTimer = 0;
    [SerializeField] bool inGlide = false;

    protected override void OnAwake()
    {
        ccData = motor.characterController.GetData();
    }

    //STRICKTLY FORBIDDEN TO USE IT FOR MOVEMENT!!!!!!!!
    //BUT YOU CAN USE IT FOR TIMERS!
    private void Update()
    {
        timer -= Time.deltaTime;
        if (Input.GetKeyDown(debugJumpKey) && allowCoyoteTime && !jumpedThisFrame && !inGlide)
            timer = coyoteSeconds;
        jumpedThisFrame = false;
    }

    public override void OnUpdate(MotorState state)
    {
        if (Input.GetKeyDown(debugJumpKey) && !inGlide)
            timer = coyoteSeconds;
        jumpedThisFrame = false;

        if (timer > 0)
        {
            if (!inGlide)
                glideTimer = glideTime;
            inGlide = true;
        }


        if (inGlide)
            Glide();
        else
            ccData.SetFullHeight(motor.characterController);

    }

    public void Glide()
    {
        timer = 0;
        jumpedThisFrame = true;
        glideTimer-=Time.deltaTime;
        if (glideTimer <= 0)
            inGlide = false;

        ccData.SetHalfHeight(motor.characterController);
    }
}

public static class CharacterControllerHelper
{
    public static CharacterControllerData GetData(this CharacterController controller) => 
        new CharacterControllerData(controller.center, controller.radius, controller.height);

    public static void SetFullHeight(this CharacterControllerData data, CharacterController controller)
    {
        controller.center = data.startCenter;
        controller.height = data.startHeight;
    }

    public static void SetHalfHeight(this CharacterControllerData data, CharacterController controller)
    {
        controller.center = data.halfedCenter;
        controller.height = data.halfedHeight;
    }
}

public struct CharacterControllerData
{
    Vector3 _startCenter;
    float _startRadius;
    float _startHeight;

    Vector3 _halfedCenter;
    float _halfedHeight;

    public Vector3 startCenter => _startCenter;
    public float startRadius => _startRadius;
    public float startHeight => _startHeight;

    public Vector3 halfedCenter => _halfedCenter;
    public float halfedHeight => _halfedHeight;

    public CharacterControllerData(Vector3 startCenter,  float startRadius, float startHeight)
    {
        _startCenter = startCenter;
        _startRadius = startRadius;
        _startHeight = startHeight;
        _halfedCenter = startCenter * 0.5f;
        _halfedHeight = startHeight * 0.5f;
    }
}
