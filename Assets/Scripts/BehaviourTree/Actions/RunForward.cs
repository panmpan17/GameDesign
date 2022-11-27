using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Run Forward")]
    public class RunForward : ActionNode
    {
        public float Speed;
        public Timer Timer;
        public AnimationCurveReference SpeedCurve;

        public ValueWithEnable<float> LimitDistance;
        private Vector3 _startPosition;


        protected override void OnStart()
        {
            _startPosition = context.transform.position;
            Timer.Reset();
        }

        protected override void OnStop()
        {
            context.rigidbody.velocity = new Vector3(0, context.rigidbody.velocity.y, 0);
        }

        protected override State OnUpdate()
        {
            if (Timer.UpdateEnd)
            {
                return State.Success;
            }

            Vector3 originVelocity = context.rigidbody.velocity;

            if (LimitDistance.Enable)
            {
                if (Vector3.Distance(_startPosition, context.transform.position) >= LimitDistance.Value)
                    return State.Success;
            }

            Vector3 velocity = SpeedCurve.Value.Evaluate(Timer.Progress) * Speed * context.transform.forward;
            originVelocity.x = velocity.x;
            originVelocity.z = velocity.z;
            context.rigidbody.velocity = velocity;
            return State.Running;
        }
    }
}