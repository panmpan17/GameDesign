using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Game/Inventory")]
public class Inventory : ScriptableObject
{
    [System.NonSerialized]
    private int _coreCount;
    public int CoreCount { get => _coreCount;
        set {
            _coreCount = value;
            CoreEvent.Invoke(_coreCount);
        }
    }
    public EventReference CoreEvent;

    [System.NonSerialized]
    private int _appleCount;
    public int AppleCount { get => _appleCount;
        set {
            _appleCount = value;
            AppleEvent.Invoke(_appleCount);
        }
    }
    public EventReference AppleEvent;

    public EventReference FlowerEvent;

    [System.NonSerialized]
    private bool _hasFlower;
    public bool HasFlower { get => _hasFlower;
        set {
            _hasFlower = value;
            FlowerEvent.Invoke(value);
        }
    }
}
