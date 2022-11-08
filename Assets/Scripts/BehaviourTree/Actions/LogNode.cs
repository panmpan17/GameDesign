using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Log")]
    public class LogNode : ActionNode
    {
        [Input]
        public BehaviourPort Input;

        public string Message;

        protected override void OnStart() { }
        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            Debug.Log(Message);
            return State.Success;
        }
    }
}