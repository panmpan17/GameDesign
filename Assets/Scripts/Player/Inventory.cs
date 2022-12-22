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

    public EventReference FlowerEvent;

    public void ChangeCoreCount(int amount)
    {
        if (amount == 0) return;
        CoreCount += amount;
        CoreEvent.Invoke(CoreCount);
    }

    public void ChangeAppleCount(int amount)
    {
        if (amount == 0) return;
        AppleCount += amount;
        AppleEvent.Invoke(AppleCount);
    }
}
