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
    public float forwardSpeed;
    public float jumpForce;
    public AnimationCurveReference jumpForceCurve;
    public Timer jumpTimer;

    [Header("Landing")]
    public bool landingMoveforawrd;
    public float extraGravity = 0;
    public LayerMask grounLayer;

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
            if (landingMoveforawrd)
                MoveForawrd(false);

            if (extraGravity != 0)
            {
                context.rigidbody.velocity += Physics.gravity * extraGravity * Time.deltaTime;
            }

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
        MoveForawrd(true);

        if (jumpTimer.UpdateEnd)
        {
            jumpTimer.Reset();
            _landing = true;
            context.slimeBehaviour.OnCollisionEnterEvent += OnLand;

            if (!landingMoveforawrd)
            {
                context.rigidbody.velocity = new Vector3(0, context.rigidbody.velocity.y, 0);
            }
        }
    }

    void MoveForawrd(bool addJumpVelocty)
    {
        Vector3 velocity = context.transform.forward * forwardSpeed;
        if (addJumpVelocty)
            velocity += Vector3.up * jumpForce * jumpForceCurve.Value.Evaluate(jumpTimer.Progress);
        else
            velocity.y = context.rigidbody.velocity.y;
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
