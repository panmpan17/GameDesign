using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Wait")]
    public class WaitNode : ActionNode
    {
        [Input]
        public float duration = 1;
        float startTime;

        private float _duration;

        protected override void OnStart()
        {
            startTime = 0;

            _duration = GetInputValue<float>("duration", duration);
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            startTime += Time.deltaTime;
            if (startTime > _duration)
            {
                return State.Success;
            }
            return State.Running;
        }
    }
}