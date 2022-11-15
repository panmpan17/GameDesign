using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace XnodeBehaviourTree
{
    [NodeTint("#FF5154")]
    [CreateNodeMenu("BehaviourTree/Root")]
    public class RootNode : AbstractBehaviourNode
    {
        [Output(connectionType: ConnectionType.Override)]
        public BehaviourPort Output;

        [System.NonSerialized]
        private AbstractBehaviourNode _child;

        public override void OnInitial()
        {
            NodePort port = GetOutputPort("Output");
            if (port.IsConnected)
                _child = (AbstractBehaviourNode)port.Connection.node;
        }

        protected override void OnStart() {}
        protected override void OnStop() {}

        protected override State OnUpdate()
        {
            return _child.Update();
        }
    }

    [System.Serializable]
    public class BehaviourPort { }
}