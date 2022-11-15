using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Check Target In Radius")]
    public class CheckTargetRadiusNode : ActionNode
    {
        public ValueWithEnable<float> Min;
        public ValueWithEnable<float> Max;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        bool IsRange()
        {
            float sqrMagnitude = (blackboard.TargetPosition - context.transform.position).sqrMagnitude;
            if (Max.Enable && sqrMagnitude > Max.Value * Max.Value)
                return false;

            if (Min.Enable && sqrMagnitude < Min.Value * Min.Value)
                return false;

            return true;
        }

        protected override State OnUpdate()
        {
            return IsRange() ? State.Success : State.Failure;
        }

        public override void DrawGizmos(Transform transform)
        {
            if (Min.Enable)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, Min.Value);
            }
            if (Max.Enable)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, Max.Value);
            }
        }
    }
}