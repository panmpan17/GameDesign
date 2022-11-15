using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace MPack
{
    public class IfElseNode : AbstractNode
    {
        [Input(typeConstraint: TypeConstraint.Strict)]
        public NodeEmptyIO Input;
        [Input(typeConstraint: TypeConstraint.Strict)]
        public bool BooleanValue;
        [Output(connectionType: ConnectionType.Override, typeConstraint: TypeConstraint.Strict)]
        public NodeEmptyIO True;
        [Output(connectionType: ConnectionType.Override, typeConstraint: TypeConstraint.Strict)]
        public NodeEmptyIO False;


        public override void Proccess()
        {
            NodePort nodePort = GetInputPort("BooleanValue");

            if (nodePort.IsConnected)
            {
                AbstractNode absNode = (AbstractNode)nodePort.Connection.node;
                absNode.PrepareValue();
                BooleanValue = nodePort.GetInputValue<bool>();
            }

            nodePort = GetOutputPort(BooleanValue ? "True" : "False");
            if (nodePort.IsConnected)
            {
                nextNode = (AbstractNode)nodePort.Connection.node;
            }
        }
    }
}