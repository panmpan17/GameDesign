using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Set Player As Target")]
    public class SetPlayerAsTarget : ActionNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            blackboard.TargetPosition = context.slimeBehaviour.PlayerTarget.position;
            return State.Success;
        }
    }
}