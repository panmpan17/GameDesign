using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using MPack;


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

    public void StartFresh()
    {
        Data = new SaveData();
        openedTreasureChestUUIDs.Clear();
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

    public void FinalizeData()
    {
        Data.OpenedTreasureChestUUIDs = openedTreasureChestUUIDs.ToArray();
        Data.FinishedStreeFightUUIDs = finishedStreetFightUUIDs.ToArray();

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

        // TODO: mission solved, mark remove from the map

        public string[] OpenedTreasureChestUUIDs;
        public string[] FinishedStreeFightUUIDs;


        public string[] VariableIntNames;
        public int[] VariableIntValues;

        public string[] VariableFloatNames;
        public float[] VariableFloatValues;

        public string[] VariableBoolNames;
        public bool[] VariableBoolValues;
    }
}
