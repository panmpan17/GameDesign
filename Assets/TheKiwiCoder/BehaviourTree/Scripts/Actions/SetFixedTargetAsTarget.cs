using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

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
