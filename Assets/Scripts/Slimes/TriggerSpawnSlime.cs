using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using DigitalRuby.Tween;

public class TriggerSpawnSlime : MonoBehaviour, ITriggerFire
{
    [SerializeField]
    private TransformPointer spawnPointsPointer;
    [SerializeField]
    private GameObject[] slimePrefabs;
    [SerializeField]
    private GameObjectPoolReference locationIndictePrefab;
    [SerializeField]
    private float delay;

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

                GameObject prefab = slimePrefabs[Random.Range(0, slimePrefabs.Length)];
                SpawnPrefab(slimePrefabs[Random.Range(0, slimePrefabs.Length)], worldPosition, transform.rotation);
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

        GameObject indicator = null;
        if (spawnAtRaycastPoint)
        {
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer.Value))
            {
                position = hit.point;

                if (locationIndictePrefab)
                {
                    indicator = locationIndictePrefab.Get();
                    indicator.transform.SetPositionAndRotation(position + hit.normal * 0.05f, Quaternion.LookRotation(hit.normal, Vector3.up));
                }
            }
        }

        if (delay > 0)
        {
            StartCoroutine(DelayExecute(prefab, position, rotation, indicator));
        }
        else
            SpawnSlimeAndDoAnimation(prefab, position, rotation, indicator);
    }

    private void SpawnSlimeAndDoAnimation(GameObject prefab, Vector3 position, Quaternion rotation, GameObject indicator)
    {
        GameObject newSlime = Instantiate(prefab, position + Vector3.down, rotation);
        spawnSlimesList.List.Add(newSlime);

        // Slime come to surface animation
        var slimeBehaviourTree = newSlime.GetComponent<XnodeBehaviourTree.BehaviourTreeRunner>();
        slimeBehaviourTree.enabled = false;
        var rigidbody = newSlime.GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        if (indicator)
            locationIndictePrefab.Put(indicator);

        newSlime.Tween(newSlime, 0, 1, 1f, TweenScaleFunctions.Linear,
            (tweenAction) =>
            {
                newSlime.transform.position = Vector3.Lerp(position + Vector3.down, position, tweenAction.CurrentValue);
            },
            (tweenAction) =>
            {
                newSlime.transform.position = position;
                rigidbody.isKinematic = false;
                slimeBehaviourTree.enabled = true;
            });
    }

    private IEnumerator DelayExecute(GameObject prefab, Vector3 position, Quaternion rotation, GameObject indicator)
    {
        yield return new WaitForSeconds(delay);
        SpawnSlimeAndDoAnimation(prefab, position, rotation, indicator);
    }


    #region Editor
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
#endregion
}
