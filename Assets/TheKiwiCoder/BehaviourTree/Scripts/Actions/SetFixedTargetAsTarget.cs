using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

#if UNITY_EDITOR
[NodeTitleName("設固定點為目標")]
#endif
public class SetFixedTargetAsTarget : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        blackboard.TargetPosition = context.slimeBehaviour.FixedTarget.position;
        return State.Success;
    }
}
