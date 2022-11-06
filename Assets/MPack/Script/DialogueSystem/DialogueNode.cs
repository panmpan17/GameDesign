using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace MPack
{
    public class DialogueNode : AbstractNode
    {
        [Input]
        public NodeEmptyIO Input;
        [Output(connectionType: ConnectionType.Override)]
        public NodeEmptyIO Output;

        [TextArea]
        public string content;

        public override void Proccess()
        {
            if (status == Status.Block)
            {
                nextNode = GetOutputNode("Output");
                status = Status.Continue;
                return;
            }
            else
            {
                DialogueGraph dialogueGraph = (DialogueGraph)graph;
                dialogueGraph.DialogueInterpreter.ChangeToDialogue(this);

                status = Status.Block;
            }
        }
    }


    [System.Serializable]
    public class NodeEmptyIO { }
}