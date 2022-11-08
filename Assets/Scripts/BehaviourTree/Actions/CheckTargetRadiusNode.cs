using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Check Target In Radius")]
    public class CheckTargetRadiusNode : ActionNode
    {
        [SerializeField]
        private ValueWithEnable<float> min;
        [SerializeField]
        private ValueWithEnable<float> max;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        bool IsRange()
        {
            float sqrMagnitude = (blackboard.TargetPosition - context.transform.position).sqrMagnitude;
            if (max.Enable && sqrMagnitude > max.Value * max.Value)
                return false;

            if (min.Enable && sqrMagnitude < min.Value * min.Value)
                return false;

            return true;
        }

        protected override State OnUpdate()
        {
            return IsRange() ? State.Success : State.Failure;
        }

        public override void DrawGizmos(Transform transform)
        {
            if (min.Enable)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, min.Value);
            }
            if (max.Enable)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, max.Value);
            }
        }
    }
}