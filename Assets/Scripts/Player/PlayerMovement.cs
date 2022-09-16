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

    [Header("Paramater")]
    [SerializeField]
    private FloatReference mouseXSensitive;
    [SerializeField]
    private FloatReference mouseYSensitive;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private Timer waitJumpTimer;
    [SerializeField]
    private float rollSpeed;
    [SerializeField]
    private AnimationCurve rollSpeedCurve;

    public event System.Action OnJump;
    public event System.Action OnJumpEnd;

    public event System.Action OnRoll;
    public event System.Action OnRollEnd;

    private Vector3 _velocity = Vector3.zero;
    private float _yVelocity = 0;

    private bool _walking = false;
    private bool _jumping = false;
    private bool _liftFromGround = false;

    private bool _rolling = false;
    private Vector3 _rollDirection = Vector3.zero;

    public bool IsWalking => _walking;
    public float AngleLerpValue { get; private set; }

    void Awake()
    {
        input.OnJump += Jump;
        input.OnRoll += Roll;
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
            FaceWithFollowTarget();

            Vector3 acceleration = transform.right * input.MovementAxis.x + transform.forward * input.MovementAxis.y;

            _velocity = acceleration * walkSpeed;
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
            OnRollEnd?.Invoke();
        }
    }

    void HandlePhysic()
    {
        if (smartGroundDetect.IsGrounded)
        {
            if (_jumping)
            {
                if (_liftFromGround)
                {
                    _jumping = false;
                    OnJumpEnd?.Invoke();
                }
            }
            else
                _yVelocity = 0;
        }
        else
        {
            _yVelocity += Physics.gravity.y * Time.deltaTime;

            if (_jumping && !_liftFromGround)
            {
                _liftFromGround = true;
            }
        }

        _velocity.y = _yVelocity;
        characterController.Move(_velocity * Time.deltaTime);
    }

    void FaceWithFollowTarget()
    {
        transform.rotation = Quaternion.Euler(0, followTarget.transform.eulerAngles.y, 0);
        followTarget.localEulerAngles = new Vector3(followTarget.transform.localEulerAngles.x, 0, 0);
    }

    void Jump()
    {
        if (_jumping || !smartGroundDetect.IsGrounded)
        {
            waitJumpTimer.Reset();
            return;
        }
        
        _jumping = true;
        _liftFromGround = false;
        _yVelocity = jumpForce;

        OnJump?.Invoke();
    }

    void Roll()
    {
        if (!behaviour.CursorFocued)
            return;
        if (_jumping || !smartGroundDetect.IsGrounded)
            return;
        if (_rolling)
            return;

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
            // followTarget.localEulerAngles = new Vector3(followTarget.transform.localEulerAngles.x, 0, 0);
        }

        OnRoll?.Invoke();
    }
}
