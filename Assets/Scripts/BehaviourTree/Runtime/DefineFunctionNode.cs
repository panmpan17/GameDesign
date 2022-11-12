using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Define Function")]
    [NodeTint("#AA68BE")]
    public class DefineFunctionNode : AbstractBehaviourNode
    {
        [Output(connectionType: ConnectionType.Override)]
        public BehaviourPort Output;

        public string FunctionName;

        private AbstractBehaviourNode _child;

        public override void OnInitial()
        {
            NodePort port = GetOutputPort("Output");
            if (port.IsConnected)
                _child = (AbstractBehaviourNode)port.Connection.node;
        }

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (_child == null)
                return State.Success;
            return _child.Update();
        }
    }
}