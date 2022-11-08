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

#if UNITY_EDITOR
        [TextArea] public string description;
#endif
        public bool drawGizmos = false;


        public override void OnInitial() { }
    }
}