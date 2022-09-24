using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

#if UNITY_EDITOR
[NodeTitleName("觸發射擊")]
#endif
public class ShootTrigger : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        context.slimeBehaviour.TriggerFire();
        return State.Success;
    }
}
