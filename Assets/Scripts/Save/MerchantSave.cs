using UnityEngine;


public class MerchantSave : MonoBehaviour
{
    [SerializeField]
    private Merchant[] merchants;
    [SerializeField]
    private SaveDataReference saveDataReference;
    [SerializeField]
    private EventReference saveDataExtractEvent;
    [SerializeField]
    private EventReference saveDataRestoreEvent;

    void OnEnable()
    {
        saveDataExtractEvent.InvokeEvents += OnSaveDataExtract;
        saveDataRestoreEvent.InvokeEvents += OnSaveDataRestore;
    }

    void OnDisable()
    {
        saveDataExtractEvent.InvokeEvents -= OnSaveDataExtract;
        saveDataRestoreEvent.InvokeEvents -= OnSaveDataRestore;
    }

    void OnSaveDataExtract()
    {
        foreach (Merchant merchant in merchants)
            merchant.OnSaveDataExtract(saveDataReference);
    }

    void OnSaveDataRestore()
    {
        foreach (Merchant merchant in merchants)
            merchant.OnSaveDataRestore(saveDataReference);
    }
}