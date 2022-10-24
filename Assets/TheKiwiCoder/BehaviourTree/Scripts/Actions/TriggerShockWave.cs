using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class TriggerShockWave : ActionNode
{
    [SerializeField]
    private float forceSize;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        context.slimeBehaviour.TriggerImpluse(forceSize);
        return State.Success;
    }
}
