using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using XNode;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Input/Random")]
    public class RandomRange : Node
    {
        public RangeReference Range;

        [Output]
        public float Output;

        // public override void ProcessData()
        // {
        //     Output = Range.PickRandomNumber();
        //     Debug.Log(Output);
        // }

        public override object GetValue(XNode.NodePort port)
        {
            Output = Range.PickRandomNumber();
            return Output;
        }
    }
}