using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[PreferComponent(typeof(StateSetterModule))]
public class Motor : MonoBehaviour, IRegistry
{
    [SerializeField] CameraRig _registeredRig;
    [SerializeField] Transform _model;

    Vector3 _velocity;

    [SerializeField] MotorState _motorState = MotorState.Grounded;
    CharacterController _characterController;
    public CharacterController characterController { get => _characterController; set => _characterController = value; }

    float _lastAngle = 0;
    Vector3 _lastNormal = Vector3.up;

    public Vector3 lastNormal { get => _lastNormal; }

    public CameraRig cameraRig { get => _registeredRig; }

    public MotorState motorState { get => _motorState; }

    public Transform model { get => _model; }

    public Vector3 charVelocity { get => characterController.velocity; }
    public Vector3 getVelocity { get => _velocity; }
    public Vector3 getSanitizedVelocity { get => new Vector3(_velocity.x, 0, _velocity.z); }
    public Dictionary<Type, List<object>> _registry { get; set; } = new Dictionary<Type, List<object>>();

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void AddVelocity(Vector3 velocity)
    {
        _velocity += velocity;
    }

    public void AddDrag(Vector3 drag)
    {
        _velocity = new Vector3(SimpleDrag(_velocity.x, drag.x), SimpleDrag(_velocity.y, drag.y), SimpleDrag(_velocity.z, drag.z));
    }

    float SimpleDrag(float start, float drag)
    {
        if (start == 0 || drag == 0) return start;
        if (start > 0)
        {
            float calc = start - drag;
            if (calc < 0) return 0;
            return calc;
        }
        else
        {
            float calc = start + drag;
            if (calc > 0) return 0;
            return calc;
        }
    }

    public void ResetYVel()
    {
        _velocity.y = 0;
    }

    public void RemoveFall()
    {
        if (_velocity.y <= -0.1f) _velocity.y = 0;
    }

    public void StopInTracks()
    {
        _velocity = Vector3.zero;
    }

    private void Update()
    {
        Pretick();
        TickModules();
        Tick();
    }

    private void LateUpdate()
    {
        Pretick();
        this.Loop<MotorModule>((module) => module?.DoLateUpdate());
    }

    void TickModules()
    {
        this.Loop<MotorModule>((module) => module?.DoUpdate());
    }

    void Pretick()
    {
        this.Loop<StateSetterModule>((module) => module?.DoUpdate());
    }

    public void OverrideState(MotorState state)
    {
        _motorState = state;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Physics.Raycast(hit.point, Vector3.down, out RaycastHit hit2, characterController.skinWidth + 0.1f))
        {
            _lastNormal = hit2.normal;
            _lastAngle = Vector3.Angle(Vector3.up, _lastNormal);
        }
        else
        {
            _lastNormal = Vector3.up;
        }
    }

    void Tick()
    {
        Move(_velocity);

        if (_velocity.y > 0 && _characterController.velocity.y == 0) _velocity.y = 0;
    }

    /// <summary>
    /// DONT INCLUDE DELTA TIME!
    /// </summary>
    /// <param name="direction"></param>
    public void Move(Vector3 direction)
    {
        Physics.SyncTransforms();
        _characterController.Move(direction * Time.deltaTime);
        Physics.SyncTransforms();
    }
}
