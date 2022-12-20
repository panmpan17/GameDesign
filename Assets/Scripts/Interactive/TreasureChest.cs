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
    [SerializeField]
    private Animator animator;

    private bool _opened;

    public void Open()
    {
        if (_opened)
            return;

        _opened = true;

        StartCoroutine(C_Animate());
    }

    public IEnumerator C_Animate()
    {
        animator.enabled = true;

        WaitForSeconds miliSecond = new WaitForSeconds(0.1f);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0)
        {
            yield return miliSecond;
        }

        // TODO: Show inventory UI
        if (appleGain.Enable)
            playerInventory.ChangeAppleCount(appleGain.Value);
        if (coreGain.Enable)
            playerInventory.ChangeCoreCount(coreGain.Value);
    }
}
