using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

namespace TheKiwiCoder {
    #if UNITY_EDITOR
    [NodeTitleName("重複")]
    #endif
    public class Repeat : DecoratorNode {

        public ValueWithEnable<int> repeatLimit;
        private int _repeatCount;

        public bool restartOnSuccess = true;
        public bool restartOnFailure = false;

        protected override void OnStart() {
            _repeatCount = 0;
        }

        protected override void OnStop() {

        }

        protected override State OnUpdate() {
            switch (child.Update()) {
                case State.Running:
                    break;
                case State.Failure:
                    if (restartOnFailure) {
                        if (repeatLimit.Enable)
                        {
                            if (++_repeatCount >= repeatLimit.Value)
                                return State.Success;
                        }
                        return State.Running;
                    } else {
                        return State.Failure;
                    }
                case State.Success:
                    if (restartOnSuccess) {
                        if (repeatLimit.Enable)
                        {
                            if (++_repeatCount >= repeatLimit.Value)
                                return State.Success;
                        }
                        return State.Running;
                    } else {
                        return State.Success;
                    }
            }
            return State.Running;
        }
    }

    
}
