using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapRegister : MonoBehaviour
{
    [SerializeField]
    private MinimapMarker marker;

    void Awake()
    {
        MinimapControl.ins.Register(transform.position, marker);
    }
}
