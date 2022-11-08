using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    public class InterruptSelectNode : SelectNode
    {
        protected override State OnUpdate()
        {
            int previous = _current;
            base.OnStart();
            var status = base.OnUpdate();
            if (previous != _current)
            {
                if (children[previous].state == State.Running)
                {
                    children[previous].Abort();
                }
            }

            return status;
        }
    }
}