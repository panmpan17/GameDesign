using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private TransformPointer targetPointer;

    private Transform Target => targetPointer.Target ? targetPointer.Target : target;

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
        if (updateMode != UpdateMode.FixedUpdate) return;
        Trigger();
    }

    void Trigger()
    {
        Transform t = Target;
        if (updatePosition)
        {

            Vector3 position = transform.position;
            if (!positionLockX) position.x = t.position.x;
            if (!positionLockY) position.y = t.position.y;
            if (!positionLockZ) position.z = t.position.z;
            transform.position = position;
        }

        if (updateRotation)
        {
            Vector3 euler = transform.rotation.eulerAngles;
            if (!rotationLockX) euler.x = t.rotation.eulerAngles.x;
            if (!rotationLockY) euler.y = t.rotation.eulerAngles.y;
            if (!rotationLockZ) euler.z = t.rotation.eulerAngles.z;
            euler += offset;
            transform.rotation = Quaternion.Euler(euler);
        }
    }
}
