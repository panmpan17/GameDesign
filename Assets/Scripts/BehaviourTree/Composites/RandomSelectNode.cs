using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Composite/Random Select")]
    public class RandomSelectNode : AbstractCompositeNode
    {
        protected int current;
        protected bool _needDecide;

        protected override void OnStart()
        {
            _needDecide = true;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (_needDecide)
            {
                _needDecide = false;
                List<int> leftIndexes = new List<int>();
                for (int i = 0; i < children.Length; i++)
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