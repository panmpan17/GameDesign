using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class SimpleSlime : MonoBehaviour
{
    [SerializeField]
    private new Rigidbody rigidbody;

    [SerializeField]
    private float rotateSpeed;

    [Header("Jump")]
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private AnimationCurve jumpForceCurve;
    [SerializeField]
    private Timer jumpTimer;
    [SerializeField]
    private RangeReference jumpWaitTimeRange;
    private Timer _jumpWaitTimer;

    [Header("Move")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Transform target;
    [SerializeField]
    [Layer]
    private int groundLayer; 

    [Header("Shoot")]
    [SerializeField]
    private BulletTrigger bulletTrigger;
    [SerializeField]
    private bool shootWhenLand;

    private SlimeState _state;
    private enum SlimeState { None, Jump, Land }

    private SlimeCore[] _cores;

    void Awake()
    {
        _cores = GetComponentsInChildren<SlimeCore>();
        for (int i = 0; i < _cores.Length; i++)
        {
            _cores[i].OnDamageEvent += OnCoreDamage;
        }

        _jumpWaitTimer = new Timer(jumpWaitTimeRange.PickRandomNumber());
    }

    void Update()
    {
        switch (_state)
        {
            case SlimeState.None:
                Vector3 delta = target.position - transform.position;
                Quaternion destinationRotation = Quaternion.Euler(0, Quaternion.LookRotation(target.position - transform.position).eulerAngles.y, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, destinationRotation, rotateSpeed * Time.deltaTime);

                if (_jumpWaitTimer.UpdateEnd)
                {
                    _jumpWaitTimer.Reset();
                    _jumpWaitTimer.TargetTime = jumpWaitTimeRange.PickRandomNumber();

                    _state = SlimeState.Jump;
                }
                break;

            case SlimeState.Jump:
                if (jumpTimer.UpdateEnd)
                {
                    jumpTimer.Reset();
                    _state = SlimeState.Land;
                }

                Vector3 velocity = Vector3.up * jumpForce * jumpForceCurve.Evaluate(jumpTimer.Progress);
                velocity += transform.forward * moveSpeed;
                rigidbody.velocity = velocity;
                break;

            case SlimeState.Land:
                break;
        }
    }

    void OnCoreDamage()
    {
        for (int i = 0; i < _cores.Length; i++)
        {
            if (_cores[i].gameObject.activeSelf)
            {
                return;
            }
        }

        var arrows = GetComponentsInChildren<Arrow>();
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].transform.SetParent(null);
            arrows[i].gameObject.SetActive(false);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_state == SlimeState.Land && collision.gameObject.layer == groundLayer)
        {
            _state = SlimeState.None;
            
            if (shootWhenLand)
                bulletTrigger.Trigger();
        }
    }
}
