using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Set Fixed As Target")]
    public class SetFixedPointAsTarget : ActionNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            blackboard.TargetPosition = context.slimeBehaviour.FixedTarget.position;
            return State.Success;
        }
    }
}