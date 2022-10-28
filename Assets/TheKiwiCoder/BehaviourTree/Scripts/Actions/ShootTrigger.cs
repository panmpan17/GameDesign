using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using MPack;

#if UNITY_EDITOR
[NodeTitleName("觸發射擊")]
#endif
public class ShootTrigger : ActionNode
{
    public ValueWithEnable<int> TriggerGroupIndex;
    public ValueWithEnable<int> CarriedParameter;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
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
