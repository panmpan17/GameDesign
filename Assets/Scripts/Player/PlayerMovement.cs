using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class PlayerMovement : MonoBehaviour
{
    [Header("Other components")]
    [SerializeField]
    private InputInterface input;
    [SerializeField]
    private PlayerBehaviour behaviour;
    [SerializeField]
    private new PlayerAnimation animation;

    [SerializeField]
    private CharacterController characterController;
    public CharacterController CharacterController => characterController;

    [SerializeField]
    private SmartGroundDetect smartGroundDetect;
    [SerializeField]
    private Transform followTarget;
    [SerializeField]
    private TransformPointer followTargetPointer;

    [Header("Paramater")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private FloatReference drawBowSlowDown;
    [SerializeField]
    private float turnSpeed;
    [Tooltip("球體旋轉角度, 下面到中間時從0變成360, 所以下會比上多")]
    [SerializeField]
    private float aimDownLimit = 320;
    [SerializeField]
    private float aimUpLimit = 40;

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

    [Header("Slope Sliding")]
    [SerializeField]
    private bool willSlopSliding;
    [SerializeField]
    private float slopeSlideSpeed;
    private Vector3 slopeNormal;

    public bool IsSlopSliding {
        get {
            RaycastHit hit;
            if (!IsGrounded || !Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, smartGroundDetect.Layers))
                return false;
            
            slopeNormal = hit.normal;
            return Vector3.Angle(slopeNormal, Vector3.up) > characterController.slopeLimit;
        }
    }


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
        input = GetComponent<InputInterface>();
        input.OnJump += OnJump;
        input.OnRoll += OnRoll;

        behaviour.OnDeath += OnDeath;

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

        followTarget.transform.rotation *= Quaternion.AngleAxis(input.LookAxis.x, Vector3.up);

        followTarget.transform.rotation *= Quaternion.AngleAxis(input.LookAxis.y, Vector3.right);

        Vector3 angles = followTarget.transform.localEulerAngles;
        angles.z = 0;

        if (angles.x > 180 && angles.x < aimDownLimit)
            angles.x = aimDownLimit;
        else if (angles.x < 180 && angles.x > aimUpLimit)
            angles.x = aimUpLimit;

        AngleLerpValue = Mathf.InverseLerp(aimDownLimit, 360 + aimUpLimit, angles.x < 180 ? angles.x + 360: angles.x);

        followTarget.transform.localEulerAngles = angles;
    }

    void HandleWalking()
    {
        if (!behaviour.CursorFocued)
            return;
        if (behaviour.IsDead)
            return;

        _walking = input.HasMovementAxis;

        if (_walking)
        {
            Vector3 acceleration = followTarget.right * input.MovementAxis.x + followTarget.forward * input.MovementAxis.y;
            acceleration.y = 0;

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
                Quaternion destination = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(acceleration, Vector3.up), turnSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, destination.eulerAngles.y, 0);
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
        if (IsSlopSliding)
        {
            if ((slopeNormal.x > 0 && _velocity.x <= 0) || (slopeNormal.x < 0 && _velocity.x >= 0))
                _velocity.x = slopeNormal.x * slopeSlideSpeed;
            if ((slopeNormal.z > 0 && _velocity.z <= 0) || (slopeNormal.z < 0 && _velocity.z >= 0))
                _velocity.z = slopeNormal.z * slopeSlideSpeed;
            _yVelocity += Physics.gravity.y * Time.deltaTime;
            return;
        }

        if (!_jumping)
        {
            _yVelocity = 0;
            return;
        }

        if (_jumping && !_liftFromGround)
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
    }

    void OnJump()
    {
        if (!behaviour.CursorFocued)
            return;
        if (behaviour.IsDead)
            return;
        if (_rolling || _jumping || !IsGrounded)
        {
            waitRollTimer.Running = false;
            waitJumpTimer.Reset();
            return;
        }
        if (IsSlopSliding)
            return;

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
        if (behaviour.IsDead)
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
        if (!input.HasMovementAxis)
        {
            FaceWithFollowTarget();
            _rollDirection = followTarget.forward;
            _rollDirection.y = 0;
            _rollDirection.Normalize();
        }
        else
        {
            Quaternion previousRotation = followTarget.rotation;

            Vector3 acceleration = followTarget.right * input.MovementAxis.x + followTarget.forward * input.MovementAxis.y;
            _rollDirection = (followTarget.right * input.MovementAxis.x + followTarget.forward * input.MovementAxis.y).normalized;
            // transform.rotation = Quaternion.LookRotation(_rollDirection, Vector3.up);

            Quaternion destination = Quaternion.LookRotation(_rollDirection, Vector3.up);
            transform.rotation = Quaternion.Euler(0, destination.eulerAngles.y, 0);

            followTarget.rotation = previousRotation;
        }

        OnRollEvent?.Invoke();
    }

    // public void Freeze()
    // {
    //     _freezed = true;

    //     waitJumpTimer.Running = false;
    //     waitRollTimer.Running = false;
    //     _velocity = Vector3.zero;

    //     _walking = false;
    //     _jumping = false;
    //     _liftFromGround = false;
    // }

    // public void Unfreeze()
    // {
    // }

    void OnDeath()
    {
        waitJumpTimer.Running = false;
        waitRollTimer.Running = false;
        _velocity = Vector3.zero;

        _walking = false;
        _jumping = false;
        _liftFromGround = false;
    }
}
