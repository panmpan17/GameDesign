using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    #if UNITY_EDITOR
    [NodeTitleName("打斷選擇器")]
    #endif
    public class InterruptSelector : Selector {
        protected override State OnUpdate() {
            int previous = current;
            base.OnStart();
            var status = base.OnUpdate();
            if (previous != current) {
                if (children[previous].state == State.Running) {
                    children[previous].Abort();
                }
            }

            return status;
        }
    }
}