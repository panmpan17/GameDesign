using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using XNode;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Input/Random Int")]
    public class RandomIntRange : Node
    {
        public IntRangeReference IntRange;

        [Output]
        public int Output;

        public override object GetValue(XNode.NodePort port)
        {
            Output = IntRange.PickRandomNumber();
            return Output;
        }
    }
}