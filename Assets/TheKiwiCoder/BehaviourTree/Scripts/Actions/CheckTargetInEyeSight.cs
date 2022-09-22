using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CheckTargetInEyeSight : ActionNode
{
    [SerializeField]
    private float senseRange;
    [SerializeField]
    private LayerMask raycastLayers;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    bool IsTargetInView()
    {
        if ((blackboard.TargetPosition - context.transform.position).sqrMagnitude > senseRange * senseRange)
            return false;

        if (Physics.Linecast(context.transform.position, blackboard.TargetPosition, raycastLayers))
            return false;

        return true;
    }

    protected override State OnUpdate() {
        return IsTargetInView() ?  State.Success : State.Failure;
    }

    public override void DrawGizmos(Transform transform)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, senseRange);
    }
}
