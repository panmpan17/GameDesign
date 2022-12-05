using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

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


    public void BuyMerchandise(int choiceIndex)
    {
        nextNode = GetOutputNode("Output");
        status = Status.Finished;

        Merchant.Merchandise merchandise = Merchant.Merchandises[choiceIndex];
        Merchant.BuyCount[choiceIndex]++;

        if (merchandise.BowUpgrade)
        {
            GameObject.FindWithTag(PlayerBehaviour.Tag).GetComponent<PlayerBehaviour>().UpgradeBow(merchandise.BowUpgrade);
            GameObject.Find("HUD").GetComponent<PlayerStatusHUD>().UnlockBowUpgrade(merchandise.BowUpgrade);
        }
    }

    public void Skip()
    {
        nextNode = GetOutputNode("Output");
        status = Status.Finished;
    }
}
