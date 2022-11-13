using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

#if UNITY_EDITOR
[NodeTitleName("觸發震波")]
#endif
public class TriggerShockWave : ActionNode
{
    public float forceSize;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        context.slimeBehaviour.TriggerImpluse(forceSize);
        return State.Success;
    }
}
