using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class PlayerMovement : MonoBehaviour
{
    [Header("Other components")]
    [SerializeField]
    private PlayerInput input;
    [SerializeField]
    private PlayerBehaviour behaviour;
    [SerializeField]
    private new PlayerAnimation animation;

    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private SmartGroundDetect smartGroundDetect;
    [SerializeField]
    private Transform followTarget;
    [SerializeField]
    private TransformPointer followTargetPointer;

    [Header("Paramater")]
    [SerializeField]
    private FloatReference mouseXSensitive;
    [SerializeField]
    private FloatReference mouseYSensitive;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private FloatReference drawBowSlowDown;
    [SerializeField]
    private float turnSpeed;

    [Header("Jump")]
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private Timer waitJumpTimer;

    [Header("Roll")]
    [SerializeField]
    private float rollSpeed;
    [SerializeField]
    private AnimationCurve rollSpeedCurve;
    [SerializeField]
    private Timer waitRollTimer;


    public event System.Action OnJumpEvent;
    public event System.Action OnJumpEndEvent;
    public event System.Action OnRejumpEvent;

    public event System.Action OnRollEvent;
    public event System.Action OnRollEndEvent;

    private Vector3 _velocity = Vector3.zero;
    private float _yVelocity = 0;

    private bool _walking = false;
    private bool _jumping = false;
    private bool _liftFromGround = false;

    private bool _rolling = false;
    private Vector3 _rollDirection = Vector3.zero;
    public bool IsRolling => _rolling;

    public bool IsWalking => _walking;
    public float AngleLerpValue { get; private set; }

    public bool IsGrounded => characterController.isGrounded || smartGroundDetect.IsGrounded;

    void Awake()
    {
        input.OnJump += OnJump;
        input.OnRoll += OnRoll;

        waitJumpTimer.Running = false;
        waitRollTimer.Running = false;

        followTargetPointer.Target = followTarget;
    }

    void Update()
    {
        if (_rolling)
            HandleRolling();
        else
        {
            HandleAimRotation();
            HandleWalking();
        }

        if (waitJumpTimer.Running)
            waitJumpTimer.Running = !waitJumpTimer.UpdateEnd;
        if (waitRollTimer.Running)
            waitRollTimer.Running = !waitRollTimer.UpdateEnd;

        HandlePhysic();
    }

    void HandleAimRotation()
    {
        if (!behaviour.CursorFocued)
            return;

        followTarget.transform.rotation *= Quaternion.AngleAxis(input.LookAxis.x * mouseXSensitive.Value, Vector3.up);

        followTarget.transform.rotation *= Quaternion.AngleAxis(input.LookAxis.y * mouseYSensitive.Value, Vector3.right);

        Vector3 angles = followTarget.transform.localEulerAngles;
        angles.z = 0;

        if (angles.x > 180 && angles.x < 340)
            angles.x = 340;
        else if (angles.x < 180 && angles.x > 40)
            angles.x = 40;

        AngleLerpValue = Mathf.InverseLerp(340, 400, angles.x < 180 ? angles.x + 360: angles.x);

        followTarget.transform.localEulerAngles = angles;
    }

    void HandleWalking()
    {
        if (!behaviour.CursorFocued)
            return;

        _walking = input.MovementAxis.sqrMagnitude > 0.01f;

        if (_walking)
        {
            // FaceWithFollowTarget();

            Vector3 acceleration = followTarget.right * input.MovementAxis.x + followTarget.forward * input.MovementAxis.y;

            if (behaviour.IsDrawingBow)
                _velocity = acceleration * walkSpeed * drawBowSlowDown.Value;
            else
                _velocity = acceleration * walkSpeed;


            if (behaviour.IsDrawingBow)
            {
                FaceWithFollowTarget();
            }
            else
            {
                Quaternion previousRotation = followTarget.rotation;
                Vector3 faceRotationDelta = acceleration;
                faceRotationDelta.y = 0;
                faceRotationDelta.Normalize();
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(faceRotationDelta, Vector3.up), turnSpeed * Time.deltaTime);
                followTarget.rotation = previousRotation;
            }
        }
        else
        {
            _velocity.x = _velocity.z = 0;

            if (behaviour.IsDrawingBow)
                FaceWithFollowTarget();
        }
    }

    void HandleRolling()
    {
        float progress = animation.RollAnimationProgress;
        _velocity = _rollDirection * (rollSpeed * rollSpeedCurve.Evaluate(progress));

        if (progress >= 1)
        {
            _rolling = false;
            FaceWithFollowTarget();
            OnRollEndEvent?.Invoke();

            if (waitJumpTimer.Running) Jump();
            if (waitRollTimer.Running) Roll();
        }
    }

    void HandlePhysic()
    {
        if (IsGrounded)
            HandleGroundedPhysic();
        else
        {
            _yVelocity += Physics.gravity.y * Time.deltaTime;

            if (_jumping && !_liftFromGround) _liftFromGround = true;
        }

        _velocity.y = _yVelocity;
        characterController.Move(_velocity * Time.deltaTime);
    }

    void HandleGroundedPhysic()
    {
        if (!_jumping)
        {
            _yVelocity = 0;
            return;
        }

        if (!_liftFromGround)
            return;

        if (waitJumpTimer.Running)
        {
            Jump();
            return;
        }

        _jumping = false;
        OnJumpEndEvent?.Invoke();

        if (waitRollTimer.Running)
            Roll();
    }

    void FaceWithFollowTarget()
    {
        transform.rotation = Quaternion.Euler(0, followTarget.transform.eulerAngles.y, 0);
        followTarget.localEulerAngles = new Vector3(followTarget.transform.localEulerAngles.x, 0, 0);

        // Quaternion previousRotation = followTarget.rotation;
        // transform.rotation = Quaternion.RotateTowards(
        //     transform.rotation,
        //     Quaternion.Euler(0, followTarget.transform.eulerAngles.y, 0), turnSpeed * Time.deltaTime);
        // followTarget.rotation = previousRotation;
    }

    void OnJump()
    {
        if (!behaviour.CursorFocued)
            return;
        if (_rolling || _jumping || !IsGrounded)
        {
            waitRollTimer.Running = false;
            waitJumpTimer.Reset();
            return;
        }

        Jump();
    }

    void Jump()
    {
        _liftFromGround = false;
        _yVelocity = jumpForce;

        if (_jumping)
        {
            OnRejumpEvent?.Invoke();
        }
        else
        {
            _jumping = true;
            OnJumpEvent?.Invoke();
        }
    }

    void OnRoll()
    {
        if (!behaviour.CursorFocued)
            return;
        if (_rolling || _jumping || !IsGrounded)
        {

            waitJumpTimer.Running = false;
            waitRollTimer.Reset();
            return;
        }

        Roll();
    }

    void Roll()
    {
        _rolling = true;
        if (input.MovementAxis.sqrMagnitude < 0.01f)
        {
            FaceWithFollowTarget();
            _rollDirection = followTarget.forward;
            _rollDirection.y = 0;
            _rollDirection.Normalize();
        }
        else
        {
            Quaternion previousRotation = followTarget.rotation;

            _rollDirection = (transform.right * input.MovementAxis.x + transform.forward * input.MovementAxis.y).normalized;
            transform.rotation = Quaternion.LookRotation(_rollDirection, Vector3.up);

            followTarget.rotation = previousRotation;
        }

        OnRollEvent?.Invoke();
    }
}
