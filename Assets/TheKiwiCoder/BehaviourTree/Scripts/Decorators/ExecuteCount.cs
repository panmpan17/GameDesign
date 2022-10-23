using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ExecuteCount : DecoratorNode
{
    [SerializeField]
    private int limit;
    private int _count = 0;

    protected override void OnStart() {
        _count++;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (_count > limit)
        {
            _count = 0;
            return State.Failure;
        }

        return child.Update();
    }
}
