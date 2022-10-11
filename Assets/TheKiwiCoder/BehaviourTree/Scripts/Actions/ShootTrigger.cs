using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using MPack;

#if UNITY_EDITOR
[NodeTitleName("觸發射擊")]
#endif
public class ShootTrigger : ActionNode
{
    public ValueWithEnable<int> TriggerGroupIndex;

    protected override void OnStart() {
        Debug.Log("shoot");
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (TriggerGroupIndex.Enable)
            context.slimeBehaviour.TriggerFireGroup(TriggerGroupIndex.Value);
        else
            context.slimeBehaviour.TriggerFire();
        return State.Success;
    }
}
