using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class TreasureChest : MonoBehaviour
{
    [SerializeField]
    private ValueWithEnable<int> appleGain;
    [SerializeField]
    private ValueWithEnable<int> coreGain;
    [SerializeField]
    private Inventory playerInventory;

    private bool _opened;

    public void Open()
    {
        if (_opened)
            return;

        _opened = true;

        if (appleGain.Enable)
            playerInventory.ChangeAppleCount(appleGain.Value);
        if (coreGain.Enable)
            playerInventory.ChangeCoreCount(coreGain.Value);

        gameObject.SetActive(false);
    }
}
