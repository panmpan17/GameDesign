using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;


#if UNITY_EDITOR
[NodeTitleName("定義")]
#endif
public class DefineFunctionNode : Node
{
    public Node child;

    public string FunctionName;

    protected override void OnStart() {}

    protected override void OnStop() {}

    protected override State OnUpdate()
    {
        if (child == null)
            return State.Success;
        return child.Update();
    }

    public override Node Clone()
    {
        DefineFunctionNode node = Instantiate(this);
        if (child)
            node.child = child.Clone();
        return node;
    }
}
