using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Composite/Stage Control")]
    public class StageControlNode : AbstractCompositeNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            // TODO: if stage index is wrong, act as selector, pick out the right stage
            State result = children[blackboard.Stage].Update();

            return result;
        }
    }
}