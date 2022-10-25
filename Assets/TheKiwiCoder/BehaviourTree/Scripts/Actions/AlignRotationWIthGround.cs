using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class AlignRotationWIthGround : ActionNode
{
    private static int groundLayers;

    void OnEnable()
    {
        groundLayers = LayerMask.GetMask("Ground");
    }

    protected override void OnStart() {
        if (Physics.Raycast(context.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 2, groundLayers))
        {
            if (Physics.Raycast(context.transform.position + context.transform.forward + Vector3.up, Vector3.down, out RaycastHit hit2, 2, groundLayers))
            {
                // context.transform.up = hit.normal;
                context.transform.LookAt(hit2.point, hit.normal);
                // hit
            }
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
