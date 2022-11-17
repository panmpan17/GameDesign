using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Jump Forward To Height")]
    public class JumpForwardToHeightNode : ActionNode
    {
        public float forwardSpeed;
        public Timer Timer;
        public AnimationCurveReference HeightCurve;
        public float Height;

        [Header("Landing")]
        public bool landingMoveforawrd;
        public float extraGravity = 0;
        public LayerMask grounLayer;

        [System.NonSerialized]
        private bool _landing = false;
        [System.NonSerialized]
        private bool _landed = false;

        private float _positionY;

        protected override void OnStart()
        {
            context.animator?.SetTrigger("Jump");
            Timer.Reset();
            _landing = _landed = false;

            _positionY = context.transform.position.y;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
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

            if (Timer.UpdateEnd)
            {
                Timer.Reset();
                _landing = true;
                context.slimeBehaviour.OnCollisionEnterEvent += OnLand;

                if (!landingMoveforawrd)
                {
                    context.rigidbody.velocity = Vector3.zero;
                }
            }

            // (context.transform.position.y - _positionY) >= LimitJumpHeight.Value)
            // {
            //     _landing = true;
            //     context.slimeBehaviour.OnCollisionEnterEvent += OnLand;

            //     if (!landingMoveforawrd)
            //     {
            //         context.rigidbody.velocity = new Vector3(0, context.rigidbody.velocity.y, 0);
            //     }
        }

        void MoveForawrd(bool addJumpVelocty)
        {
            Vector3 velocity = context.transform.forward * forwardSpeed;
            if (addJumpVelocty)
            {
                float destinateHeight = _positionY + (Height * HeightCurve.Value.Evaluate(Timer.Progress));
                velocity.y += (destinateHeight - context.transform.position.y) * (1 / Time.deltaTime);
            }
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
}