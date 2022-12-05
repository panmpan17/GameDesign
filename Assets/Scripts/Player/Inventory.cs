using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Game/Inventory")]
public class Inventory : ScriptableObject
{
    [System.NonSerialized]
    public int CoreCount;
    public EventReference CoreEvent;
    [System.NonSerialized]
    public int AppleCount;
    public EventReference AppleEvent;

    public void ChangeCoreCount(int amount)
    {
        CoreCount += amount;
        CoreEvent.Invoke(CoreCount);
    }
}
