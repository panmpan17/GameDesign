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

    [Header("Colddown")]
    [SerializeField]
    private bool hasColddown;
    [SerializeField]
    private Timer colddownTimer;

    private bool _inRange;

    private GameObjectList spawnSlimes;

    void Awake()
    {
        spawnSlimes = ScriptableObject.CreateInstance<GameObjectList>();

        for (int i = 0; i < spawnSlimeTriggers.Length; i++)
        {
            spawnSlimeTriggers[i].SetSpawnSlimeList(spawnSlimes);
        }

        colddownTimer.Running = false;
    }

    void Update()
    {
        if (colddownTimer.Running)
        {
            if (colddownTimer.UpdateEnd)
                colddownTimer.Running = false;
            return;
        }

        if (!senseTimer.UpdateEnd)
            return;
        senseTimer.Reset();

        // TODO: add colddown time

        float sqrMagnitude = (playerTransform.Target.position - transform.position).sqrMagnitude;
        if (_inRange)
        {
            if (sqrMagnitude < despawnRadius * despawnRadius)
                return;

            _inRange = false;

            if (hasColddown && spawnSlimes.AliveCount <= 0)
            {
                colddownTimer.Reset();
            }
            spawnSlimes?.DestroyAll();
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
