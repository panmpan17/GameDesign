using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName="Game/GameObject 列表")]
public class GameObjectList : ScriptableObject
{
    public List<GameObject> List;

    public ValueWithEnable<int> CountLimit;

    public int AliveCount {
        get {
            int count = 0;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i] == null)
                {
                    List.RemoveAt(i);
                    i--;
                }
                else
                    count++;
            }
            return count;
        }
    }
}
