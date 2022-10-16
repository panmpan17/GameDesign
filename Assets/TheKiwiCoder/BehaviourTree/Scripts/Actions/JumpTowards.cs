using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using MPack;

#if UNITY_EDITOR
[NodeTitleName("往前跳")]
#endif
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
    [SerializeField]
    private LayerMask grounLayer;

    [System.NonSerialized]
    private bool _landing = false;
    [System.NonSerialized]
    private bool _landed = false;

    protected override void OnStart()
    {
        context.animator?.SetTrigger("Jump");
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
            context.slimeBehaviour.OnCollisionEnterEvent += OnLand;
        }

        Vector3 velocity = Vector3.up * jumpForce * jumpForceCurve.Value.Evaluate(jumpTimer.Progress);
        velocity += context.transform.forward * forwardSpeed;
        context.rigidbody.velocity = velocity;
    }

    void OnLand(Collision collision)
    {
        // if (collision.gameObject.layer ==  grounLayer)
        if (grounLayer == (grounLayer | (1 << collision.gameObject.layer)))
        {
            context.slimeBehaviour.OnCollisionEnterEvent -= OnLand;
            _landed = true;
            context.animator?.SetTrigger("Land");
        }
    }
}
