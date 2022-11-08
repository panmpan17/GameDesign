using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;



namespace XnodeBehaviourTree
{
    [CreateAssetMenu(menuName="Game/BehaviourTreeGraph")]
    public class BehaviourTreeGraph : NodeGraph
    {
        [System.NonSerialized]
        private RootNode _rootNode;
        [System.NonSerialized]
        private AbstractBehaviourNode.State _state;

        void OnInitial()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                AbstractBehaviourNode node;
                try
                {
                    node = (AbstractBehaviourNode)nodes[i];
                }
                catch (System.InvalidCastException) { continue; }

                if (node is RootNode)
                {
                    _rootNode = (RootNode)node;
                }

                node.OnInitial();
            }
        }

        public AbstractBehaviourNode.State Update()
        {
            if (_rootNode.state == AbstractBehaviourNode.State.Running)
            {
                _state = _rootNode.Update();
            }
            return _state;
        }

        public override XNode.NodeGraph Copy()
        {
            NodeGraph graph = base.Copy();
            ((BehaviourTreeGraph)graph).OnInitial();
            return graph;
        }
    }
}