using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayerEvent : MonoBehaviour
{
    [SerializeField]
    private BowParameter bowUpgrade;

    public void UpgradeBow()
    {
        GameObject.FindWithTag(PlayerBehaviour.Tag).GetComponent<PlayerBehaviour>().UpgradeBow(bowUpgrade);
        // GameObject.Find("HUD").GetComponent<PlayerStatusHUD>().UnlockBowUpgrade(bowUpgrade);
    }
}
