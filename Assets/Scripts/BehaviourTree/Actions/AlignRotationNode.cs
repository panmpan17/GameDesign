using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Align Rotation")]
    public class AlignRotationNode : ActionNode
    {
        private static int groundLayers;

        public override void OnInitial()
        {
            groundLayers = LayerMask.GetMask("Ground");
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (Physics.Raycast(context.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 2, groundLayers))
            {
                if (Physics.Raycast(context.transform.position + context.transform.forward + Vector3.up, Vector3.down, out RaycastHit hit2, 2, groundLayers))
                {
                    context.transform.LookAt(hit2.point, hit.normal);
                }
            }
            return State.Success;
        }
    }
}