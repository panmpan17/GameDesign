using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform targetPointer;

    [SerializeField]
    private UpdateMode updateMode;
    [SerializeField]
    private bool lockX;
    [SerializeField]
    private bool lockY;
    [SerializeField]
    private bool lockZ;

    private enum UpdateMode { LateUpdate, FixedUpdate, Manual }

    void LateUpdate()
    {
        if (updateMode != UpdateMode.LateUpdate) return;
        Trigger();
    }

    void FixedUpdate()
    {
        if (updateMode != UpdateMode.LateUpdate) return;
        Trigger();
    }

    void Trigger()
    {
        Vector3 position = target.position;
        if (lockX) position.x = transform.position.x;
        if (lockY) position.y = transform.position.y;
        if (lockZ) position.z = transform.position.z;
        transform.position = position;
    }
}
