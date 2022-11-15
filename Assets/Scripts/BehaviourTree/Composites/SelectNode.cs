using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Composite/Select")]
    public class SelectNode : AbstractCompositeNode
    {
        protected int _current;

        protected override void OnStart()
        {
            _current = 0;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            for (int i = _current; i < children.Length; ++i)
            {
                _current = i;
                var child = children[_current];

                switch (child.Update())
                {
                    case State.Running:
                        return State.Running;
                    case State.Success:
                        return State.Success;
                    case State.Failure:
                        continue;
                }
            }

            return State.Failure;
        }
    }
}