using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Face Target")]
    public class FaceTargetNode : ActionNode
    {
        private static int groundLayers;

        public float RotateSpeed;
        public bool RaycastPoint;

        public bool LockRotationXZAxis;


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
            Quaternion destinationRotation = UseRaycastPointToFindDesinationRotation();

            // Vector3 direction = blackboard.TargetPosition - context.transform.position;
            // direction.Normalize();
            // Debug.DrawRay(context.transform.position, direction * 10 , Color.white, 1f);

            // float angle = Mathf.Acos(Vector3.Dot(direction, context.transform.forward)) * Mathf.Rad2Deg;
            // Quaternion destinationRotation = Quaternion.AngleAxis(angle, context.transform.forward);

            // Debug.DrawRay(context.transform.position, context.transform.forward * 10, Color.green, 1f);
            // Debug.DrawRay(context.transform.position, destinationRotation * Vector3.forward, Color.red, 1f);

            destinationRotation = Quaternion.RotateTowards(context.transform.rotation, destinationRotation, RotateSpeed * Time.deltaTime);

            float angleDifference = Quaternion.Angle(context.transform.rotation, destinationRotation);

            if (LockRotationXZAxis)
                destinationRotation = Quaternion.Euler(0, destinationRotation.eulerAngles.y, 0);

            context.transform.rotation = destinationRotation;

            return angleDifference < 0.01f ? State.Success : State.Running;
        }

        Quaternion UseRaycastPointToFindDesinationRotation()
        {
            Quaternion destinationRotation = Quaternion.LookRotation(
                            blackboard.TargetPosition - context.transform.position, context.transform.up);

            Vector3 origin = context.transform.position + (destinationRotation * (Vector3.forward * 0.1f));

            if (RaycastPoint && Physics.Raycast(blackboard.TargetPosition, Vector3.down, out RaycastHit hit, 10, groundLayers))
            {
                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit2, 10, groundLayers))
                {
                    destinationRotation = Quaternion.LookRotation(hit.point - hit2.point, context.transform.up);
                }
            }

            if (LockRotationXZAxis)
                destinationRotation = Quaternion.Euler(0, destinationRotation.eulerAngles.y, 0);

            return destinationRotation;
        }
    }
}