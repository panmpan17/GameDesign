using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class TriggerSpawnSlime : MonoBehaviour, ITriggerFire
{
    [SerializeField]
    private TransformPointer spawnPointsPointer;
    [SerializeField]
    private GameObject[] slimePrefabs;

    [SerializeField]
    private GameObjectList spawnSlimesList;

    [Header("Circle Setting")]
    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private float radius;
    [SerializeField]
    [Min(1)]
    private int segmentCount = 1;

    [Header("Raycast Ground")]
    [SerializeField]
    private bool spawnAtRaycastPoint;
    [SerializeField]
    private float raycastDistance;
    [SerializeField]
    private LayerMaskReference groundLayer;

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
        SpawnPrefabInSegmentPoints();
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
                SpawnPrefab(prefab, points[i].position, points[i].rotation);
                spawnLeft--;
            }
        }
    }

    void SpawnPrefabInSegmentPoints()
    {
        int spawnLeft = spawnCount;
        for (int i = 0; spawnLeft > 0 && i < _segmentPoints.Length; i++)
        {
            float chance = (float)spawnLeft / (_segmentPoints.Length - i);
            float randomValue = Random.value;
            if (randomValue <= chance)
            {
                Vector3 worldDirection = transform.TransformDirection(_segmentPoints[i]);
                Vector3 worldPosition = transform.position + worldDirection * radius;

                if (spawnAtRaycastPoint)
                {
                    if (Physics.Raycast(worldPosition, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer.Value))
                    {
                        worldPosition = hit.point;
                    }
                }

                GameObject prefab = slimePrefabs[Random.Range(0, slimePrefabs.Length)];
                SpawnPrefab(prefab, worldPosition, transform.rotation);
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
                Vector3 worldPosition = transform.position + worldDirection * radius;
                SpawnPrefab(prefab, worldPosition, transform.rotation);
                spawnLeft--;
            }
        }
    }

    void SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (spawnSlimesList.CountLimit.Enable && spawnSlimesList.List.Count >= spawnSlimesList.CountLimit.Value)
            return;

        GameObject newSlime = Instantiate(prefab, position, rotation);
        spawnSlimesList.List.Add(newSlime);
        newSlime.GetComponent<XnodeBehaviourTree.BehaviourTreeRunner>().OnDeath.AddListener(delegate {
            spawnSlimesList.List.Remove(newSlime);
        });
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
