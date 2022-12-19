using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using DigitalRuby.Tween;


public abstract class AbstractSpawnSlime : MonoBehaviour, ITriggerFire
{
    public event System.Action OnSlimeSpawnedCallback;
    public abstract void SetSpawnSlimeList(GameObjectList list);
    public abstract void TriggerFire();
    public abstract void TriggerFireWithParameter(int parameter);

    public virtual void ResetFight() {}

    protected void TriggerOnSlimeSpawnedCallback()
    {
        OnSlimeSpawnedCallback?.Invoke();
        OnSlimeSpawnedCallback = null;
    }
}


public class TriggerSpawnSlime : AbstractSpawnSlime
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


    public override void SetSpawnSlimeList(GameObjectList list) => spawnSlimesList = list;

    public override void TriggerFire()
    {
        SpawnPrefabInSegmentPoints();
    }

    public override void TriggerFireWithParameter(int parameter)
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
        if (spawnSlimesList && spawnSlimesList.ReachedLimit)
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
        var slimeBehaviour = newSlime.GetComponent<SlimeBehaviour>();
        slimeBehaviour.DisableTreeRunner();
        newSlime.GetComponent<Rigidbody>().isKinematic = true;

        if (indicator)
            locationIndictePrefab.Put(indicator);

        slimeBehaviour.EmergeFromTheGround(position);

        TriggerOnSlimeSpawnedCallback();
    }

    private IEnumerator DelayExecute(GameObject prefab, Vector3 position, Quaternion rotation, GameObject indicator)
    {
        yield return new WaitForSeconds(delay);
        SpawnSlimeAndDoAnimation(prefab, position, rotation, indicator);
    }
}
