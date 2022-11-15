using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace XnodeBehaviourTree
{
    [NodeTint("#D08600")]
    public abstract class AbstractCompositeNode : AbstractBehaviourNode
    {
        [Input]
        public BehaviourPort Input;

        [Output]
        public BehaviourPort Output;

        [System.NonSerialized]
        public AbstractBehaviourNode[] children;

        public override void OnInitial()
        {
            NodePort port = GetOutputPort("Output");

            List<NodePort> oppsites = port.GetConnections();
            children = new AbstractBehaviourNode[oppsites.Count];

            oppsites.Sort((port1, port2) => {
                return Mathf.RoundToInt(port1.node.position.y - port2.node.position.y);
            });

            for (int i = 0; i < oppsites.Count; i++)
            {
                children[i] = (AbstractBehaviourNode)oppsites[i].node;
            }

            return;
        }

        // public override Node Clone()
        // {
        //     CompositeNode node = Instantiate(this);
        //     node.children = children.ConvertAll(c => c.Clone());
        //     return node;
        // }
    }
}