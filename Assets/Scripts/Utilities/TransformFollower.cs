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

    [Header("Position")]
    [SerializeField]
    private bool updatePosition;
    [SerializeField]
    private bool positionLockX;
    [SerializeField]
    private bool positionLockY;
    [SerializeField]
    private bool positionLockZ;

    [Header("Rotation")]
    [SerializeField]
    private bool updateRotation;
    [SerializeField]
    private bool rotationLockX;
    [SerializeField]
    private bool rotationLockY;
    [SerializeField]
    private bool rotationLockZ;
    [SerializeField]
    private Vector3 offset;

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
        if (updatePosition)
        {
            Vector3 position = transform.position;
            if (!positionLockX) position.x = target.position.x;
            if (!positionLockY) position.y = target.position.y;
            if (!positionLockZ) position.z = target.position.z;
            transform.position = position;
        }

        if (updateRotation)
        {
            Vector3 euler = transform.rotation.eulerAngles;
            if (!rotationLockX) euler.x = target.rotation.eulerAngles.x;
            if (!rotationLockY) euler.y = target.rotation.eulerAngles.y;
            if (!rotationLockZ) euler.z = target.rotation.eulerAngles.z;
            euler += offset;
            transform.rotation = Quaternion.Euler(euler);
        }
    }
}
