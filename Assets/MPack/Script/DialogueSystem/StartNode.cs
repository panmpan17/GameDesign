using XNode;
using UnityEngine;


namespace MPack
{
    [NodeTint(0, 0.75f, 0.1f)]
    public class StartNode : AbstractNode
    {
        [Output(connectionType: Node.ConnectionType.Override)]
        public NodeEmptyIO Output;

        public override void Proccess()
        {
            NodePort port = GetOutputPort("Output");
            if (port.IsConnected)
            {
                nextNode = (AbstractNode)port.Connection.node;
            }
        }
    }
}