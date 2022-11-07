using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using MPack;


#if UNITY_EDITOR
[NodeTitleName("觸發事件")]
#endif
public class TriggerEvent : ActionNode
{
    [SerializeField]
    private EventReference eventReference;
    [SerializeField]
    private ValueWithEnable<float> carriedFloatValue;
    [SerializeField]
    private ValueWithEnable<int> carriedIntValue;
    [SerializeField]
    private ValueWithEnable<bool> carriedBoolValue;


    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        eventReference.Invoke();

        if (carriedFloatValue.Enable)
            eventReference.Invoke(carriedFloatValue.Value);

        if (carriedIntValue.Enable)
            eventReference.Invoke(carriedIntValue.Value);

        if (carriedBoolValue.Enable)
            eventReference.Invoke(carriedBoolValue.Value);

        return State.Success;
    }
}
