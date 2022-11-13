using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Call Function")]
    [NodeTint("#AA68BE")]
    public class CallFunctionNode : ActionNode
    {
        public string FunctionName;

        private DefineFunctionNode _defineNode;

        protected override void OnStart() { }
        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (_defineNode == null)
                return State.Failure;

            return _defineNode.Update();
        }

        public void SetDefineNode(DefineFunctionNode node) => _defineNode = node;
    }
}