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

        public void OnInitial(TheKiwiCoder.Context context)
        {
            TheKiwiCoder.Blackboard blackboard = new TheKiwiCoder.Blackboard();
            List<DefineFunctionNode> defineFunctionNodes = new List<DefineFunctionNode>();

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
                else if (node is DefineFunctionNode)
                {
                    defineFunctionNodes.Add((DefineFunctionNode)node);
                }
                else if (node is CallFunctionNode)
                {
                    var callNode = (CallFunctionNode)node;
                    for (int e = 0; e < defineFunctionNodes.Count; e++)
                    {
                        if (defineFunctionNodes[e].FunctionName == callNode.FunctionName)
                        {
                            callNode.SetDefineNode(defineFunctionNodes[e]);
                            break;
                        }
                    }
                }

                node.OnInitial();
                node.context = context;
                node.blackboard = blackboard;
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
    }
}