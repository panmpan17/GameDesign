using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceSetter : MonoBehaviour
{
    [SerializeField]
    private TransformPointer transformPointer;
    [SerializeField]
    private Transform target;

    void Awake()
    {
        transformPointer.Target = target;
    }
}
