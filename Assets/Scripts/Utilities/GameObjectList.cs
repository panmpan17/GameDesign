using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName="Game/GameObject 列表")]
public class GameObjectList : ScriptableObject
{
    public List<GameObject> List = new List<GameObject>();

    public ValueWithEnable<int> CountLimit;

    public bool ReachedLimit => CountLimit.Enable && AliveCount >= CountLimit.Value;

    public int AliveCount {
        get {
            int count = 0;
            for (int i = List.Count - 1; i >= 0; i--)
            {
                if (List[i] == null) List.RemoveAt(i);
                else count++;
            }
            return count;
        }
    }

    public void DestroyAll()
    {
        while (List.Count > 0)
        {
            if (List[0])
                Destroy(List[0]);
            List.RemoveAt(0);
        }
    }
}
