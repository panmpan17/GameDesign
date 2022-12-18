using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateNodeMenu("M Pack/Open Merchant")]
public class OpenMerchantNode : AbstractNode
{
    [Input]
    public NodeEmptyIO Input;
    [Output(connectionType: ConnectionType.Override)]
    public NodeEmptyIO Output;

    public Speaker Speaker;
    [LauguageID]
    public int ContentLaguageID;

    public Merchant Merchant;

    public override void Proccess()
    {
        if (status == Status.Finished)
        {
            status = Status.Continue;
            return;
        }

        DialogueGraph dialogueGraph = (DialogueGraph)graph;
        ((DialogueUI)dialogueGraph.DialogueInterpreter).ChangeToMerchant(this);

        status = Status.Block;
    }

    public void Skip()
    {
        nextNode = GetOutputNode("Output");
        status = Status.Finished;
    }
}
