using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName="Game/Merchant")]
public class Merchant : ScriptableObject
{
    public string UUID;

    public Merchandise[] Merchandises;
    [System.NonSerialized]
    private int[] _buyCount;
    public int[] BuyCount {
        get {
            _buyCount ??= new int[Merchandises.Length];
            return _buyCount;
        }
    }

    public bool CheckMerchandiseLimitReached(int index)
    {
        Merchandise merchandise = Merchandises[index];
        return merchandise.PurchaseLimit.Enable && BuyCount[index] >= merchandise.PurchaseLimit.Value;
    }

    public void OnSaveDataExtract(SaveDataReference saveDataReference)
    {
        saveDataReference.AddMerchantRecord(UUID, _buyCount);
    }

    public void OnSaveDataRestore(SaveDataReference saveDataReference)
    {
        int[] result = saveDataReference.GetMerchantBuyCount(UUID);

        if (result != null)
            _buyCount = result;
    }

    [System.Serializable]
    public struct Merchandise
    {
        [Header("Buy")]
        public int RequireCoreCount;
        [LauguageID]
        public int NameLanguageID;

        public ValueWithEnable<int> PurchaseLimit;
        public bool DisplayWhenPurchaseLimitReached;

        [Header("Return")]
        public BowParameter BowUpgrade;
        public int AppleGain;
    }
}
