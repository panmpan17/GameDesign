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

    [SerializeField]
    private XnodeBehaviourTree.BehaviourTreeRunner[] slimes;


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

        if (slimes.Length > 0)
        {
            foreach (var slime in slimes)
            {
                spawnSlimeList.List.Add(slime.gameObject);
                slime.enabled = true;
            }

            onWaveStarted?.Invoke();
        }
    }
}
