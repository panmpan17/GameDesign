using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SetPlayerPositionAsTarget : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        blackboard.TargetPosition = context.slimeBehaviour.PlayerTarget.position;
        return State.Success;
    }
}
