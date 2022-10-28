using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawnSlime : MonoBehaviour, ITriggerFire
{
    [SerializeField]
    private TransformPointer spawnPointsPointer;
    [SerializeField]
    private GameObject[] slimePrefabs;

    [Header("Circle Setting")]
    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private float radius;
    [SerializeField]
    [Min(1)]
    private int segmentCount = 1;

    [Header("Editor Only")]
    [SerializeField]
    private bool drawDizmos;

    private Vector3[] _segmentPoints;

    void Start()
    {
        CalculateSegmentPoints();
    }

    public void TriggerFire()
    {
    }

    public void TriggerFireWithParameter(int parameter)
    {
        GameObject prefab = slimePrefabs[parameter];

        if (spawnPointsPointer)
            SpawnPrefabInSpanwPointsPointer(prefab);
        else
            SpawnPrefabInSegmentPoints(prefab);
    }

    void SpawnPrefabInSpanwPointsPointer(GameObject prefab)
    {
        int spawnLeft = spawnCount;

        Transform[] points = spawnPointsPointer.Targets;
        for (int i = 0; spawnLeft > 0 && i < points.Length; i++)
        {
            float chance = (float)spawnLeft / (points.Length - i);
            float randomValue = Random.value;
            if (randomValue <= chance)
            {
                GameObject newSlime = Instantiate(prefab, points[i].position, points[i].rotation);
                spawnLeft--;
            }
        }
    }

    void SpawnPrefabInSegmentPoints(GameObject prefab)
    {
        int spawnLeft = spawnCount;
        for (int i = 0; spawnLeft > 0 && i < _segmentPoints.Length; i++)
        {
            float chance = (float)spawnLeft / (_segmentPoints.Length - i);
            float randomValue = Random.value;
            if (randomValue <= chance)
            {
                Vector3 worldDirection = transform.TransformDirection(_segmentPoints[i]);
                Vector3 worldPostion = transform.position + worldDirection * radius;
                GameObject newSlime = Instantiate(prefab, worldPostion, transform.rotation);
                spawnLeft--;
            }
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
            delta = Quaternion.AngleAxis(rotateRadius, Vector3.up) * delta;
        }
    }

    void OnValidate()
    {
        CalculateSegmentPoints();
    }

    void OnDrawGizmosSelected()
    {
        if (!drawDizmos)
            return;
        // Gizmos.DrawRay(transform.position, transform.forward);
        if (_segmentPoints == null)
            CalculateSegmentPoints();

        Vector3 position = transform.position;
        for (int i = 0; i < _segmentPoints.Length; i++)
        {
            Vector3 worldDirection = transform.TransformDirection(_segmentPoints[i]);
            Vector3 worldPostion = position + worldDirection * radius;
            Gizmos.DrawSphere(worldPostion, 0.1f);
        }
    }
}
