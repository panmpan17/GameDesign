using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

#if UNITY_EDITOR
[NodeTitleName("面向目標")]
#endif
public class FaceTarget : ActionNode
{
    [SerializeField]
    private float rotateSpeed;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Quaternion destinationRotation = Quaternion.LookRotation(blackboard.TargetPosition - context.transform.position, context.transform.up);
        destinationRotation = Quaternion.RotateTowards(context.transform.rotation, destinationRotation, rotateSpeed * Time.deltaTime);
        float angleDifference = Quaternion.Angle(context.transform.rotation, destinationRotation);
        context.transform.rotation = destinationRotation;

        return angleDifference < 0.01f ? State.Success : State.Running;
    }
}
