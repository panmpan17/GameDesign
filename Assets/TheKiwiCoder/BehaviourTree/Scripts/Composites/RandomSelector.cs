using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TheKiwiCoder {
    #if UNITY_EDITOR
    [NodeTitleName("隨機選擇器")]
    #endif
    public class RandomSelector : CompositeNode {
        protected int current;
        protected bool _needDecide;

        protected override void OnStart() {
            // current = Random.Range(0, children.Count);
            _needDecide = true;
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            if (_needDecide)
            {
                _needDecide = false;
                List<int> leftIndexes = new List<int>();
                for (int i = 0; i < children.Count; i++)
                    leftIndexes.Add(i);

                while (leftIndexes.Count >= 0)
                {
                    int removeIndex = Random.Range(0, leftIndexes.Count);
                    int index = leftIndexes[removeIndex];
                    leftIndexes.RemoveAt(removeIndex);

                    switch (children[index].Update())
                    {
                        case State.Running:
                            current = index;
                            return State.Running;
                        case State.Success:
                            return State.Success;
                        case State.Failure:
                            continue;
                    }
                }

                return State.Failure;
            }

            return children[current].Update();
        }
    }
}