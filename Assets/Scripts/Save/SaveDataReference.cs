using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


[CreateAssetMenu(menuName="Game/Save Data")]
public class SaveDataReference : ScriptableObject
{
    [System.NonSerialized]
    public SaveData Data;

    // TODO: mission solved, mark remove from the map
    // TODO: Include variable storage

    public void SaveToFilePath(string path)
    {
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
        return true;
    }


    [System.Serializable]
    public struct SaveData
    {
        [SerializeField]
        private float[] playerPosition;
        public Vector3 PlayerPosition { get => new Vector3(playerPosition[0], playerPosition[1], playerPosition[2]); set {
            playerPosition = new float[] { value.x, value.y, value.z };
        }}

        // public int[] playerRotation;
        // public Vector3 PlayerRotation { get => new Vector3(playerRotation[])}
        public string SpawnPointName;

        public int CoreCount;
        public int AppleCount;
        public float HealthPoint;

        public bool HasFollower;
        public bool BowUpgrade1Aquired;
        public bool BowUpgrade2Aquired;
        public bool BowUpgrade3Aquired;
    }
}
