using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletRotationControl : MonoBehaviour
{
    [SerializeField]
    private BaseBullet baseBullet;

    [SerializeField]
    private bool willFaceVelocityDirection;

    [SerializeField]
    private bool willRotate;
    [SerializeField]
    private Transform meshTransform;
    [SerializeField]
    private Vector3 rotationSpeed;

    void Awake()
    {
        if (willFaceVelocityDirection)
            baseBullet.OnShoot += OnBulletShoot;
        if (!willRotate)
            enabled = false;
    }

    void FixedUdpate()
    {
        meshTransform.Rotate(rotationSpeed * Time.deltaTime);
    }

    void OnBulletShoot(Vector3 velocity)
    {
        transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
    }
}
