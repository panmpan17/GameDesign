using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;


#if UNITY_EDITOR
[NodeTitleName("呼叫")]
#endif
public class CallFunctionNode : Node
{
    public string FunctionName;
    private Node _defineNode;

    protected override void OnStart() { }
    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        if (_defineNode == null)
            return State.Failure;

        return _defineNode.Update();
    }

    public void SetDefineNode(Node node) => _defineNode = node;
}
