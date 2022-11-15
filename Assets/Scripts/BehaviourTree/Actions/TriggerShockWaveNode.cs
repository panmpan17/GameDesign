using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Set Fixed As Target")]
    public class TriggerShockWaveNode : ActionNode
    {
        public float forceSize;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            context.slimeBehaviour.TriggerImpluse(forceSize);
            return State.Success;
        }
    }
}