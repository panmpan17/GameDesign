using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayerEvent : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private BowParameter bowUpgrade;

    public void UpgradeBow()
    {
        InventoryGainUI.ins.ShowBowUpgrade(bowUpgrade, GameManager.ins.Player.UpgradeBow);
    }

    public void AddApple(int count)
    {
        inventory.ChangeAppleCount(count);
    }

    public void TakeFlowerFromInventory()
    {
        inventory.FlowerEvent.Invoke(false);
    }
}
