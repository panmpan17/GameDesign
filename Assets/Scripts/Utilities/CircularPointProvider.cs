using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularPointProvider : AbstractPointProvider
{
    [SerializeField]
    private float radius;
    [SerializeField]
    [Min(1)]
    private int segmentCount = 1;

    [Header("Editor Only")]
    [SerializeField]
    private bool drawGizmos;

    private Vector3[] _pointDeltas;

    void Awake() { CalculateSegmentPoints(); }

    void CalculateSegmentPoints()
    {
        _pointDeltas = new Vector3[segmentCount];
        Vector3 delta = transform.forward;
        float rotateRadius = 360f / (float)segmentCount;

        Vector3 selfPosition = transform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            _pointDeltas[i] = delta;
            delta = Quaternion.AngleAxis(rotateRadius, Vector3.up) * delta;
        }
    }


    public override Point[] GetPoints()
    {
        Point[] points = new Point[_pointDeltas.Length];

        Vector3 position = transform.position;
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 worldDirection = transform.TransformDirection(_pointDeltas[i]);

            points[i] = new Point {
                Poisition = position + worldDirection * radius,
                Forawrd = worldDirection,
            };
        }

        return points;
    }

    void OnValidate() { CalculateSegmentPoints(); }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;
        // Gizmos.DrawRay(transform.position, transform.forward);
        if (_pointDeltas == null)
            CalculateSegmentPoints();

        Vector3 position = transform.position;
        for (int i = 0; i < _pointDeltas.Length; i++)
        {
            Vector3 worldDirection = transform.TransformDirection(_pointDeltas[i]);
            Vector3 worldPostion = position + worldDirection * radius;
            Gizmos.DrawSphere(worldPostion, 0.1f);
            Gizmos.DrawRay(worldPostion, worldDirection);
        }
    }
}
