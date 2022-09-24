using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

#if UNITY_EDITOR
[NodeTitleName("檢查目標在範圍內")]
#endif
public class TargetIsInRange : ActionNode
{
    [SerializeField]
    private float radius;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        bool inRange = (blackboard.TargetPosition - context.transform.position).sqrMagnitude < radius * radius;
        return inRange ? State.Success : State.Failure;
    }

    public override void DrawGizmos(Transform transform)
    {
        Gizmos.DrawSphere(blackboard.TargetPosition, radius);
    }
}
