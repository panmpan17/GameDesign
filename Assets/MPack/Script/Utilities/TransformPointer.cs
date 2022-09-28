using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName="MPack/Transform Pointer")]
public class TransformPointer : ScriptableObject
{
    public Transform Target {
        get => _target;
        set {
            _target = value;
            OnChange?.Invoke(value);
        }
    }
    private Transform _target;
    public event System.Action<Transform> OnChange;
}
