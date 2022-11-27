using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class FightWave : MonoBehaviour
{
    [SerializeField]
    private bool waitForPreviousWave;
    [SerializeField]
    private Timer waitTimer;

    [SerializeField]
    private TriggerSpawnSlime[] spawnSlimeTriggers;


    public bool CanStart(bool allSlimesAreDead)
    {
        if (waitForPreviousWave && !allSlimesAreDead)
            return false;

        if (!waitTimer.UpdateEnd)
            return false;

        return true;
    }

    public void StartWave(GameObjectList spawnSlimeList, System.Action onWaveStarted)
    {
        for (int i = 0; i < spawnSlimeTriggers.Length; i++)
        {
            var trigger = spawnSlimeTriggers[i];
            trigger.OnSlimeSpawnedCallback += onWaveStarted;
            trigger.SetSpawnSlimeList(spawnSlimeList);
            trigger.TriggerFire();
        }
    }
}
