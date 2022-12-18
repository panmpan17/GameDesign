using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Random Position As Target")]
    public class RandomPositionAsTarget : ActionNode
    {
        public float startRadius;
        public float endRadius;


        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            Vector2 value = Random.insideUnitCircle;
            value.Normalize();
            value *= Random.Range(startRadius, endRadius);
            blackboard.TargetPosition = context.transform.position + new Vector3(value.x, 0.5f, value.y);
            return State.Success;
        }

        public override void DrawGizmos(Transform transform)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, startRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, endRadius);
        }
    }
}