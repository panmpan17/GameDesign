using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CheckPlayerInRange : ActionNode
{
    [SerializeField]
    private float min;
    [SerializeField]
    private float max;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    bool IsRange()
    {
        float sqrMagnitude = (blackboard.TargetPosition - context.transform.position).sqrMagnitude;
        if (sqrMagnitude > max * max)
            return false;

        if (sqrMagnitude < min * min)
            return false;

        return true;
    }

    protected override State OnUpdate()
    {
        return IsRange() ? State.Success : State.Failure;
    }
}
