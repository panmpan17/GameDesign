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
    private CharacterController characterController;
    [SerializeField]
    private SmartGroundDetect smartGroundDetect;
    [SerializeField]
    private Transform followTarget;

    [Header("paramater")]
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private Timer waitJumpTimer;

    public event System.Action OnJump;
    public event System.Action OnJumpEnd;

    private Vector3 _velocity = Vector3.zero;
    private float _yVelocity = 0;

    private bool _walking = false;
    private bool _jumping = false;
    private bool _liftFromGround = false;

    public bool IsWalking => _walking;
    public float AngleLerpValue { get; private set; }

    void Awake()
    {
        input.OnJump += Jump;
    }

    void Update()
    {
        HandleAimRotation();
        HandleWalking();
        HandlePhysic();
    }

    void HandleAimRotation()
    {
        if (!behaviour.CursorFocued)
            return;

        followTarget.transform.rotation *= Quaternion.AngleAxis(input.LookAxis.x * rotateSpeed, Vector3.up);

        followTarget.transform.rotation *= Quaternion.AngleAxis(input.LookAxis.y * rotateSpeed, Vector3.right);

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
}
