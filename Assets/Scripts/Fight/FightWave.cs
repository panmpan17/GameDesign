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
    private Transform[] spawnPoints;
    // [SerializeField]
    // private FightSpawnRule[] rules;
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

    public void Spawn()
    {
        for (int i = 0; i < spawnSlimeTriggers.Length; i++)
        {
            spawnSlimeTriggers[i].TriggerFire();
        }
        // for (int i = 0; i < rules.Length; i++)
        // {
        //     SpawnByRule(rules[i]);
        // }
    }

    // void SpawnByRule(FightSpawnRule rule)
    // {
    //     int amount = rule.AmountRange.PickRandomNumber();
    //     for (int i = 0; i < amount; i++)
    //     {
    //         Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
    //     }
    // }
}
