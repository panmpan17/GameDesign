using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using DigitalRuby.Tween;

public class TriggerSpawnSlime : MonoBehaviour, ITriggerFire
{
    [SerializeField]
    private GameObject[] slimePrefabs;
    [SerializeField]
    private GameObjectPoolReference locationIndictePrefab;
    [SerializeField]
    private float delay;

    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private GameObjectList spawnSlimesList;
    [SerializeField]
    private AbstractPointProvider spawnPointProvider;

    [Header("Raycast Ground")]
    [SerializeField]
    private bool spawnAtRaycastPoint;
    [SerializeField]
    private float raycastDistance;
    [SerializeField]
    private LayerMaskReference groundLayer;


    [HideInInspector, SerializeField] private float radius;
    [HideInInspector, SerializeField] private int segmentCount = 1;

    private Vector3[] _segmentPoints;
    public event System.Action OnSlimeSpawnedCallback;


    public void SetSpawnSlimeList(GameObjectList list) => spawnSlimesList = list;

    public void TriggerFire()
    {
        SpawnPrefabInSegmentPoints();
    }

    public void TriggerFireWithParameter(int parameter)
    {
        SpawnPrefabInSegmentPoints(slimePrefabs[parameter]);
    }

    void SpawnPrefabInSegmentPoints()
    {
        int spawnLeft = spawnCount;
        AbstractPointProvider.Point[] points =  spawnPointProvider.GetPoints();

        for (int i = 0; spawnLeft > 0 && i < points.Length; i++)
        {
            float chance = (float)spawnLeft / (points.Length - i);
            if (Random.value > chance)
                continue;

            GameObject prefab = slimePrefabs[Random.Range(0, slimePrefabs.Length)];
            SpawnPrefab(slimePrefabs[Random.Range(0, slimePrefabs.Length)], points[i].Poisition, Quaternion.LookRotation(points[i].Forawrd, Vector3.up));
            spawnLeft--;
        }
    }
    void SpawnPrefabInSegmentPoints(GameObject prefab)
    {
        int spawnLeft = spawnCount;
        AbstractPointProvider.Point[] points = spawnPointProvider.GetPoints();

        for (int i = 0; spawnLeft > 0 && i < points.Length; i++)
        {
            float chance = (float)spawnLeft / (points.Length - i);
            if (Random.value > chance)
                continue;

            SpawnPrefab(slimePrefabs[Random.Range(0, slimePrefabs.Length)], points[i].Poisition, Quaternion.LookRotation(points[i].Forawrd, Vector3.up));
            spawnLeft--;
        }
    }

    void SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (spawnSlimesList && spawnSlimesList.CountLimit.Enable && spawnSlimesList.List.Count >= spawnSlimesList.CountLimit.Value)
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

        if (spawnSlimesList)
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

        OnSlimeSpawnedCallback?.Invoke();
        OnSlimeSpawnedCallback = null;
    }

    private IEnumerator DelayExecute(GameObject prefab, Vector3 position, Quaternion rotation, GameObject indicator)
    {
        yield return new WaitForSeconds(delay);
        SpawnSlimeAndDoAnimation(prefab, position, rotation, indicator);
    }
}
