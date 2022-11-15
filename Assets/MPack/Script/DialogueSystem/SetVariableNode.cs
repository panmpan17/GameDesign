using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace MPack
{
    public class SetVariableNode : AbstractNode
    {
        [Input]
        public NodeEmptyIO Input;
        [Output(connectionType: ConnectionType.Override)]
        public NodeEmptyIO Output;

        [HideInInspector, VaribleName]
        public string variableName;
        [HideInInspector]
        public bool variableBoolValue;
        [HideInInspector]
        public int variableIntValue;
        [HideInInspector]
        public float variableFloatValue;

        public override void Proccess()
        {
            DialogueGraph dialogueGraph = (DialogueGraph) graph;

            DialogueGraph.VaribleType varibleType = dialogueGraph.GetVaribleType(variableName);
            switch (varibleType)
            {
                case DialogueGraph.VaribleType.Float:
                    dialogueGraph.varibles.Set(variableName, variableFloatValue);
                    break;
                case DialogueGraph.VaribleType.Int:
                    dialogueGraph.varibles.Set(variableName, variableIntValue);
                    break;
                case DialogueGraph.VaribleType.Bool:
                    dialogueGraph.varibles.Set(variableName, variableBoolValue);
                    break;
            }

            NodePort port = GetOutputPort("Output");
            if (port.IsConnected)
            {
                nextNode = (AbstractNode)port.Connection.node;
            }
        }
    }
}