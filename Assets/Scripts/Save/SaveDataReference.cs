using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using MPack;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(menuName="Game/Save Data")]
public class SaveDataReference : ScriptableObject
{
    public VariableStorage variableStorage;

    [System.NonSerialized]
    public SaveData Data;
    [System.NonSerialized]
    private List<string> openedTreasureChestUUIDs = new List<string>();
    [System.NonSerialized]
    private List<string> finishedStreetFightUUIDs = new List<string>();
    [SerializeField]
    private List<MerchantRecord> merchantRecords = new List<MerchantRecord>();

    public void StartFresh()
    {
        Data = new SaveData();
        openedTreasureChestUUIDs.Clear();
        finishedStreetFightUUIDs.Clear();
        merchantRecords.Clear();
        merchantRecords.Clear();
    }

    public void SaveToFilePath(string path)
    {
        FinalizeData();

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        FileStream stream = new FileStream(path, FileMode.Create);
        binaryFormatter.Serialize(stream, Data);
        stream.Close();
    }

    public bool ReadFromFilePath(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (!File.Exists(path))
        {
            Data = new SaveData();
            return false;
        }

        FileStream stream = new FileStream(path, FileMode.Open);
        Data = (SaveData)binaryFormatter.Deserialize(stream);
        stream.Close();

        openedTreasureChestUUIDs.Clear();
        finishedStreetFightUUIDs.Clear();
        merchantRecords.Clear();
        // openedTreasureChestUUIDs.AddRange(Data.OpenedTreasureChestUUIDs);
        finishedStreetFightUUIDs.AddRange(Data.FinishedStreeFightUUIDs);

        SetVariableUsingNameValueArray(variableStorage.Set, Data.VariableBoolNames, Data.VariableBoolValues);
        SetVariableUsingNameValueArray(variableStorage.Set, Data.VariableIntNames, Data.VariableIntValues);
        SetVariableUsingNameValueArray(variableStorage.Set, Data.VariableFloatNames, Data.VariableFloatValues);

        return true;
    }

    void SetVariableUsingNameValueArray<T>(System.Action<string, T> dictionaySetAction, string[] variableNames, T[] variableValues)
    {
        int length = variableNames.Length;
        for (int i = 0; i < length; i++)
        {
            dictionaySetAction.Invoke(variableNames[i], variableValues[i]);
        }
    }

    public void AddOpenedTreasureChest(string treasureChestUUID) => openedTreasureChestUUIDs.Add(treasureChestUUID);
    public bool TreasureChestIsOpened(string treasureChestUUID)
    {
        if (Data.OpenedTreasureChestUUIDs == null)
            return false;

        foreach (string chestUUIDRecord in Data.OpenedTreasureChestUUIDs)
        {
            if (chestUUIDRecord == treasureChestUUID)
                return true;
        }
        return false;
    }

    public void AddFinishedStreetFight(string streetFightUUID) => finishedStreetFightUUIDs.Add(streetFightUUID);
    public bool StreetFightIsFinished(string streetFightUUID)
    {
        if (Data.FinishedStreeFightUUIDs == null)
            return false;

        foreach (string fightUUIDRecord in Data.FinishedStreeFightUUIDs)
        {
            if (fightUUIDRecord == streetFightUUID)
                return true;
        }
        return false;
    }

    public void AddMerchantRecord(string merchantUUID, int[] buyCount) => merchantRecords.Add(new MerchantRecord { UUID = merchantUUID, BuyCount = buyCount });
    public int[] GetMerchantBuyCount(string merchantUUID)
    {
        if (Data.MerchantRecords == null)
            return null;

        foreach (MerchantRecord merchantRecord in Data.MerchantRecords)
        {
            if (merchantRecord.UUID == merchantUUID)
                return merchantRecord.BuyCount;
        }
        return null;
    }

    public void FinalizeData()
    {
        Data.OpenedTreasureChestUUIDs = openedTreasureChestUUIDs.ToArray();
        Data.FinishedStreeFightUUIDs = finishedStreetFightUUIDs.ToArray();
        Data.MerchantRecords = merchantRecords.ToArray();

        variableStorage.ExportBoolVariableForSerialization(out string[] boolVariableNames, out bool[] boolVariableValues);
        Data.VariableBoolNames = boolVariableNames;
        Data.VariableBoolValues = boolVariableValues;

        variableStorage.ExportIntVariableForSerialization(out string[] intVariableNames, out int[] intVariableValues);
        Data.VariableIntNames = intVariableNames;
        Data.VariableIntValues = intVariableValues;

        variableStorage.ExportFloatVariableForSerialization(out string[] floatVariableNames, out float[] floatVariableValues);
        Data.VariableFloatNames = floatVariableNames;
        Data.VariableFloatValues = floatVariableValues;
    }


#if UNITY_EDITOR
    [MenuItem("Game/Open Persistent Data Path")]
    public static void s_OpenPersistentDataPath()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
    [MenuItem("Game/Convert Binnary File To Json", true)]
    public static bool s_Validate_ConvertBinnaryFileToJson()
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, "save1"));
    }
    [MenuItem("Game/Convert Binnary File To Json")]
    public static void s_ConvertBinnaryFileToJson()
    {
        string path = Path.Combine(Application.persistentDataPath, "save1");

        var saveDataReference = ScriptableObject.CreateInstance<SaveDataReference>();

        string variableStorageUUID = AssetDatabase.FindAssets("t:VariableStorage")[0];
        saveDataReference.variableStorage = AssetDatabase.LoadAssetAtPath<VariableStorage>(AssetDatabase.GUIDToAssetPath(variableStorageUUID));
        saveDataReference.ReadFromFilePath(path);

        string stringData = JsonUtility.ToJson(saveDataReference.Data, true);
        System.IO.File.WriteAllText(path + ".json", stringData);
    }

    [MenuItem("Game/Convert Json To Binnary File", true)]
    public static bool s_Validate_ConvertJsonToBinnaryFile()
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, "save1.json"));
    }
    [MenuItem("Game/Convert Json To Binnary File")]
    public static void s_ConvertJsonToBinnaryFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "save1");
        string stringData = System.IO.File.ReadAllText(path + ".json");
        SaveDataReference.SaveData saveData = JsonUtility.FromJson<SaveDataReference.SaveData>(stringData);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        FileStream stream = new FileStream(path, FileMode.Create);
        binaryFormatter.Serialize(stream, saveData);
        stream.Close();
    }
#endif


    [System.Serializable]
    public struct SaveData
    {
        [SerializeField]
        private float[] playerPosition;
        public Vector3 PlayerPosition { get => new Vector3(playerPosition[0], playerPosition[1], playerPosition[2]); set {
            playerPosition = new float[] { value.x, value.y, value.z };
        }}

        public string SpawnPointName;

        public int CoreCount;
        public int AppleCount;
        public float HealthPoint;

        public bool HasFollower;
        public bool BowUpgrade1Aquired;
        public bool BowUpgrade2Aquired;
        public bool BowUpgrade3Aquired;

        public string[] OpenedTreasureChestUUIDs;
        public string[] FinishedStreeFightUUIDs;


        public string[] VariableIntNames;
        public int[] VariableIntValues;

        public string[] VariableFloatNames;
        public float[] VariableFloatValues;

        public string[] VariableBoolNames;
        public bool[] VariableBoolValues;

        public MerchantRecord[] MerchantRecords;
    }

    [System.Serializable]
    public struct MerchantRecord
    {
        public string UUID;
        public int[] BuyCount;
    }
}
