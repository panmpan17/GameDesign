using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace MPack
{
    public class TriggerEventNode : AbstractNode
    {
        [Input]
        public NodeEmptyIO Input;
        [Output(connectionType: ConnectionType.Override)]
        public NodeEmptyIO Output;

        public EventReference eventReference;

        public override void Proccess()
        {
            eventReference.Invoke();
            nextNode = GetOutputNode("Output");
            status = Status.Continue;
        }
    }
}