using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [NodeTint("#7E9C2A")]
    public abstract class ActionNode : AbstractBehaviourNode
    {
        [Input]
        public BehaviourPort Input;

        public bool drawGizmos = false;


        public override void OnInitial() { }
    }
}