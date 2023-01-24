using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[ExecuteInEditMode]
public class TreasureChest : MonoBehaviour
{
    [SerializeField]
    private ValueWithEnable<int> appleGain;
    [SerializeField]
    private ValueWithEnable<int> coreGain;
    [SerializeField]
    private BowParameter bowUpgrade;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClipSet openSound;

    [Header("Saving")]
    [SerializeField]
    private SaveDataReference saveDataReference;
    [SerializeField]
    private EventReference saveDataExtractEvent;
    [SerializeField]
    private EventReference saveDataRestoreEvent;
    [SerializeField]
    private string uuid;

    private bool _opened;

    void Awake()
    {
        saveDataExtractEvent.InvokeEvents += OnSaveDataExtract;
        saveDataRestoreEvent.InvokeEvents += OnSaveDataRestore;
    }

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

        if (bowUpgrade)
        {
            InventoryGainUI.ins.ShowBowUpgrade(bowUpgrade);
        }
        else
        {
            int appleAmount = appleGain.Enable ? appleGain.Value : 0;
            int coreAmount = coreGain.Enable ? coreGain.Value : 0;
            InventoryGainUI.ins.ShowInventoryGain(appleAmount, coreAmount);
        }
    }

    void OnSaveDataExtract()
    {
        if (_opened)
            saveDataReference.AddOpenedTreasureChest(uuid);
    }

    void OnSaveDataRestore()
    {
        _opened = saveDataReference.TreasureChestIsOpened(uuid);
        if (!_opened)
            return;

        // TODO: If player interact detect the treasure chest when file loaded, the interact key won't go away
        animator.enabled = true;
        animator.Update(100);
    }

    void OnDestroy()
    {
        saveDataExtractEvent.InvokeEvents -= OnSaveDataExtract;
        saveDataRestoreEvent.InvokeEvents -= OnSaveDataRestore;
    }
}
