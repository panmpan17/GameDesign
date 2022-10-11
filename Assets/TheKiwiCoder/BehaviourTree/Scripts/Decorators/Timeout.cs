using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    #if UNITY_EDITOR
    [NodeTitleName("時間到停止")]
    #endif
    public class Timeout : DecoratorNode {
        public float duration = 1.0f;
        public bool resultIsSuccess;
        float startTime;

        protected override void OnStart() {
            startTime = 0;
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            startTime += Time.deltaTime;
            if (startTime > duration) {
                return resultIsSuccess ? State.Success : State.Failure;
            }

            return child.Update();
        }
    }
}