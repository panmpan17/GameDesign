using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace MPack
{
    public abstract class AbstractNode : Node
    {
        public Status status { get; protected set; }
        // public bool isFinished { get; protected set; }
        public AbstractNode nextNode { get; protected set; }

        public enum Status {
            Continue,
            Block,
            Finished,
        }

        public abstract void Proccess();

        public virtual void PrepareValue() {}

        public AbstractNode GetOutputNode(string portName)
        {
            NodePort port = GetOutputPort(portName);
            if (port.IsConnected)
                return (AbstractNode)port.Connection.node;
            else
                return null;
        }
    }
}