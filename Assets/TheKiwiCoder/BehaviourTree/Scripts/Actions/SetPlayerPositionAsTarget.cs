using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

#if UNITY_EDITOR
[NodeTitleName("設玩家為目標")]
#endif
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
