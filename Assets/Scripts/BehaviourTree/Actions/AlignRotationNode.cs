using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Align Rotation")]
    public class AlignRotationNode : ActionNode
    {
        private static int groundLayers;

        public float RaycastDistance = 3;

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
            Vector3 higherPosition = context.transform.position;
            higherPosition.y += 0.1f;
            if (Physics.Raycast(higherPosition, Vector3.down, out RaycastHit hit, RaycastDistance, groundLayers))
            {
                if (Physics.Raycast(higherPosition + context.transform.forward, Vector3.down, out RaycastHit hit2, RaycastDistance, groundLayers))
                {
                    context.transform.LookAt(hit2.point, hit.normal);
                }
            }
            return State.Success;
        }
    }
}