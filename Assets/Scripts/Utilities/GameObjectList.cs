using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName="Game/GameObject 列表")]
public class GameObjectList : ScriptableObject
{
    public List<GameObject> List;

    public ValueWithEnable<int> CountLimit;
}
