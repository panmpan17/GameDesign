using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Wait")]
    public class WaitNode : ActionNode
    {
        public float duration = 1;
        float startTime;

        protected override void OnStart()
        {
            startTime = 0;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            startTime += Time.deltaTime;
            if (startTime > duration)
            {
                return State.Success;
            }
            return State.Running;
        }
    }
}