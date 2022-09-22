using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using MPack;

public class JumpTowards : ActionNode
{
    [SerializeField]
    private float forwardSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private AnimationCurveReference jumpForceCurve;
    [SerializeField]
    private Timer jumpTimer;

    private bool _landing;
    private bool _landed;

    protected override void OnStart()
    {
        context.animator.SetTrigger("Jump");
        jumpTimer.Reset();
        _landing = _landed = false;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (_landing)
        {
            return _landed ? State.Success : State.Running;
        }
        else
        {
            HandleJumping();
            return State.Running;
        }
    }

    void HandleJumping()
    {
        if (jumpTimer.UpdateEnd)
        {
            jumpTimer.Reset();
            _landing = true;
            context.slimeBehaviour.OnLandEvent += OnLand;
        }

        Vector3 velocity = Vector3.up * jumpForce * jumpForceCurve.Value.Evaluate(jumpTimer.Progress);
        velocity += context.transform.forward * forwardSpeed;
        context.rigidbody.velocity = velocity;
    }

    void OnLand()
    {
        context.slimeBehaviour.OnLandEvent -= OnLand;
        _landed = true;
        context.animator.SetTrigger("Land");
    }
}
