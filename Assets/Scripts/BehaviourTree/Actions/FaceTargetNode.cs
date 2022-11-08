using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Face Target")]
    public class FaceTargetNode : ActionNode
    {
        private static int groundLayers;

        [SerializeField]
        private float rotateSpeed;
        [SerializeField]
        private bool raycastPoint;


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
            Quaternion destinationRotation = Quaternion.LookRotation(
                blackboard.TargetPosition - context.transform.position, context.transform.up);

            Vector3 origin = context.transform.position + (destinationRotation * (Vector3.forward * 0.1f)) + Vector3.up;

            if (raycastPoint && Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 2, groundLayers))
            {
                destinationRotation = Quaternion.LookRotation(hit.point - context.transform.position, context.transform.up);
            }

            destinationRotation = Quaternion.RotateTowards(context.transform.rotation, destinationRotation, rotateSpeed * Time.deltaTime);

            float angleDifference = Quaternion.Angle(context.transform.rotation, destinationRotation);
            context.transform.rotation = destinationRotation;

            return angleDifference < 0.01f ? State.Success : State.Running;
        }
    }
}