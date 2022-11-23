using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetFightManager : MonoBehaviour
{
    [SerializeField]
    private PlayerDetection entranceDetect;

    [SerializeField]
    private GameObject borderWall;
    [SerializeField]
    private EventReference playerReviveEvent;
    [SerializeField]
    private GameObjectList spawnedSlimes;

    [SerializeField]
    private FightWave[] waves;
    private int _waveIndex;
    private bool _waveStarted;

    void Awake()
    {
        borderWall.SetActive(false);
        entranceDetect.OnPlayerEnterEvent += OnPlayerEnterEntrance;
    }

    void Update()
    {
        if (!_waveStarted)
            return;

        bool allSlimesAreDead = spawnedSlimes.AliveCount <= 0;

        if (_waveIndex >= waves.Length)
        {
            if (!allSlimesAreDead) return;

            borderWall.SetActive(false);
            enabled = false;
            return;
        }

        if (waves[_waveIndex].CanStart(allSlimesAreDead))
        {
            waves[_waveIndex].StartWave(spawnedSlimes, OnWaveStarted);
            _waveStarted = false;
            _waveIndex++;
        }
    }

    void OnPlayerEnterEntrance()
    {
        playerReviveEvent.InvokeEvents += ResetFight;

        entranceDetect.gameObject.SetActive(false);
        borderWall.SetActive(true);

        waves[_waveIndex].StartWave(spawnedSlimes, OnWaveStarted);
        _waveStarted = false;
        _waveIndex++;

        enabled = true;
    }

    void OnWaveStarted()
    {
        _waveStarted = true;
    }


    void ResetFight()
    {
        enabled = false;

        playerReviveEvent.InvokeEvents -= ResetFight;

        borderWall.SetActive(false);

        if (spawnedSlimes)
        {
            while (spawnedSlimes.List.Count >= 1)
            {
                Destroy(spawnedSlimes.List[0]);
                spawnedSlimes.List.RemoveAt(0);
            }
        }

        _waveIndex = 0;

        entranceDetect.gameObject.SetActive(true);
    }
}
