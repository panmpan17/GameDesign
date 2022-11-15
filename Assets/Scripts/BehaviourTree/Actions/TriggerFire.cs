using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Trigger Fire")]
    public class TriggerFire : ActionNode
    {
        public ValueWithEnable<int> TriggerGroupIndex;
        public ValueWithEnable<int> CarriedParameter;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (TriggerGroupIndex.Enable)
            {
                if (CarriedParameter.Enable)
                    context.slimeBehaviour.TriggerFireGroup(TriggerGroupIndex.Value, CarriedParameter.Value);
                else
                    context.slimeBehaviour.TriggerFireGroup(TriggerGroupIndex.Value);
            }
            else
            {
                if (CarriedParameter.Enable)
                    context.slimeBehaviour.TriggerFire(CarriedParameter.Value);
                else
                    context.slimeBehaviour.TriggerFire();
            }
            return State.Success;
        }
    }
}