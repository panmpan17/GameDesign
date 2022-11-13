using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

#if UNITY_EDITOR
[NodeTitleName("檢查目標在範圍內")]
#endif
public class TargetIsInRange : ActionNode
{
    public float radius;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Vector3 delta = blackboard.TargetPosition - context.transform.position;
        bool inRange = (delta.x * delta.x + delta.z * delta.z) < radius * radius;
        return inRange ? State.Success : State.Failure;
    }

    public override void DrawGizmos(Transform transform)
    {
        Gizmos.DrawSphere(blackboard.TargetPosition, radius);
    }
}
