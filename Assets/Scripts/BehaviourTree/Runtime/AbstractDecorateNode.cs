using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace XnodeBehaviourTree
{
    [NodeTint("#687DBE")]
    public abstract class AbstractDecorateNode : AbstractBehaviourNode
    {
        [Input]
        public BehaviourPort Input;

        [Output]
        public BehaviourPort Output;

        [System.NonSerialized] public AbstractBehaviourNode _child;


        public override void OnInitial()
        {
            NodePort port = GetOutputPort("Output");
            if (port.IsConnected)
                _child = (AbstractBehaviourNode)port.Connection.node;
        }

        public override Node Clone() {
            AbstractBehaviourNode node = Instantiate(this);
            return node;
        }
    }
}