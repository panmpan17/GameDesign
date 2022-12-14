using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Decorate/Timeout")]
    public class TimeoutNode : AbstractDecorateNode
    {
        [Input]
        public float duration = 1.0f;
        public bool resultIsSuccess;
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
                return resultIsSuccess ? State.Success : State.Failure;
            }

            return _child.Update();
        }
    }
}