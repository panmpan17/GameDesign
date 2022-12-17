using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Animator Control")]
    public class AnimatorControlNode : ActionNode
    {
        [Header("Animator")]
        public string TriggerName;
        public string BooleanName;
        public bool BooleanValue;

        [Header("Face Sprite Animation")]
        public string SpriteAnimationName;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (TriggerName != "")
            {
                context.animator.SetTrigger(TriggerName);
            }
            if (BooleanName != "")
            {
                context.animator.SetBool(BooleanName, BooleanValue);
            }

            if (SpriteAnimationName != "")
            {
                context.animationPlayer.PlayAnimation(SpriteAnimationName);
            }

            return State.Success;
        }
    }
}