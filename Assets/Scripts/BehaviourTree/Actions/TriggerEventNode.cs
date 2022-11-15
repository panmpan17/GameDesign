using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Set Fixed As Target")]
    public class TriggerEventNode : ActionNode
    {
        public EventReference eventReference;
        public ValueWithEnable<float> carriedFloatValue;
        public ValueWithEnable<int> carriedIntValue;
        public ValueWithEnable<bool> carriedBoolValue;


        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            eventReference.Invoke();

            if (carriedFloatValue.Enable)
                eventReference.Invoke(carriedFloatValue.Value);

            if (carriedIntValue.Enable)
                eventReference.Invoke(carriedIntValue.Value);

            if (carriedBoolValue.Enable)
                eventReference.Invoke(carriedBoolValue.Value);

            return State.Success;
        }
    }
}