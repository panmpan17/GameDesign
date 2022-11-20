using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class AmbushPoint : MonoBehaviour
{
    [SerializeField]
    private TriggerSpawnSlime[] spawnSlimeTriggers;

    [Header("Sense Player")]
    [SerializeField]
    private float spawnRadius;
    [SerializeField]
    private float despawnRadius;
    [SerializeField]
    private Timer senseTimer;
    [SerializeField]
    private TransformPointer playerTransform;

    private bool _inRange;

    private GameObjectList spawnSlimes;

    void Awake()
    {
        spawnSlimes = ScriptableObject.CreateInstance<GameObjectList>();
        spawnSlimes.List = new List<GameObject>();

        for (int i = 0; i < spawnSlimeTriggers.Length; i++)
        {
            spawnSlimeTriggers[i].SetSpawnSlimeList(spawnSlimes);
        }
    }

    void Update()
    {
        if (!senseTimer.UpdateEnd)
            return;
        senseTimer.Reset();

        float sqrMagnitude = (playerTransform.Target.position - transform.position).sqrMagnitude;
        if (_inRange)
        {
            if (sqrMagnitude < despawnRadius * despawnRadius)
                return;

            _inRange = false;
            while (spawnSlimes.List.Count > 0)
            {
                if (spawnSlimes.List[0])
                    Destroy(spawnSlimes.List[0]);
                spawnSlimes.List.RemoveAt(0);
            }
        }
        else
        {
            if (sqrMagnitude > spawnRadius * spawnRadius)
                return;

            _inRange = true;
            for (int i = 0; i < spawnSlimeTriggers.Length; i++)
            {
                spawnSlimeTriggers[i].TriggerFire();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, despawnRadius);
    }
}
