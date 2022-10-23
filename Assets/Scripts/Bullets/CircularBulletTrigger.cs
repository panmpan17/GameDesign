using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularBulletTrigger : MonoBehaviour, ITriggerFire
{
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private BulletType bulletType;

    [Header("Circle Setting")]
    [SerializeField]
    private float radius;
    [SerializeField]
    [Min(1)]
    private int segmentCount = 1;

    private Vector3[] _segmentPoints;

    void Start()
    {
        CalculateSegmentPoints();
    }

    public void TriggerFire()
    {
        BulletBehaviour bullet;
        Vector3 position = transform.position;
        for (int i = 0; i < _segmentPoints.Length; i++)
        {
            bullet = bulletType.Pool.Get();

            // Vector3 position = transform.TransformPoint(_segmentPoints[i].LocalPosition);
            Vector3 worldDirection = transform.TransformDirection(_segmentPoints[i]);

            if (bulletType.UseBillboardRotate)
                bullet.transform.SetPositionAndRotation(position + worldDirection * radius, BulletBillboards.ins.FaceCameraRotation);
            else
                bullet.transform.position = position + worldDirection * radius;

            bullet.Shoot(worldDirection * bulletSpeed);
        }
    }

    void CalculateSegmentPoints()
    {
        _segmentPoints = new Vector3[segmentCount];
        Vector3 delta = transform.forward;
        float rotateRadius = 360f / (float)segmentCount;

        Vector3 selfPosition = transform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            _segmentPoints[i] = delta;
            // Vector3 position = delta * radius;
            // _segmentPoints[i] = new SegmentPoint {
            //     LocalPosition = position,
            //     Direction = delta,
            // };

            delta = Quaternion.AngleAxis(rotateRadius, Vector3.up) * delta;
        }
    }

    void OnValidate()
    {
        CalculateSegmentPoints();
    }

    void OnDrawGizmosSelected()
    {
        // Gizmos.DrawRay(transform.position, transform.forward);
        if (_segmentPoints == null)
            CalculateSegmentPoints();

        Vector3 position = transform.position;
        for (int i = 0; i < _segmentPoints.Length; i++)
        {
            Vector3 worldDirection = transform.TransformDirection(_segmentPoints[i]);
            Vector3 worldPostion = position + worldDirection * radius;
            Gizmos.DrawSphere(worldPostion, 0.1f);
            Gizmos.DrawRay(worldPostion, worldDirection * bulletSpeed);
        }
    }

    public struct SegmentPoint
    {
        public Vector3 LocalPosition;
        public Vector3 Direction;
    }
}
