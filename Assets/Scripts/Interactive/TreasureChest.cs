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
    private Animator animator;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClipSet openSound;

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

        yield return new WaitForSeconds(0.5f);
        audioSource.Play(openSound);

        WaitForSeconds miliSecond = new WaitForSeconds(0.1f);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return miliSecond;
        }

        int appleAmount = appleGain.Enable ? appleGain.Value : 0;
        int coreAmount = coreGain.Enable ? coreGain.Value : 0;
        InventoryGainUI.ins.ShowInventoryGain(appleAmount, coreAmount);
    }
}
