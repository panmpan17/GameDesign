using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StreetFightManager : MonoBehaviour
{
    [SerializeField]
    private PlayerDetection entranceDetect;

    [SerializeField]
    private GameObject borderWall;
    [SerializeField]
    private EventReference playerReviveEvent;
    [SerializeField]
    private BGMClip bgmClip;

    [SerializeField]
    private GameObjectList spawnedSlimes;

    [SerializeField]
    private FightWave[] waves;
    private int _waveIndex;
    private bool _waveStarted;

    public UnityEvent OnEndEvent;

    void Awake()
    {
        borderWall.SetActive(false);
        entranceDetect.OnPlayerEnterEvent += OnPlayerEnterEntrance;

        spawnedSlimes ??= ScriptableObject.CreateInstance<GameObjectList>();
    }

    void Update()
    {
        if (!_waveStarted)
            return;

        bool allSlimesAreDead = spawnedSlimes.AliveCount <= 0;

        if (_waveIndex >= waves.Length)
        {
            if (!allSlimesAreDead) return;

            enabled = false;

            borderWall.SetActive(false);

            if (bgmClip)
                BGMPlayer.ins.ResetToBaseBGM();

            OnEndEvent.Invoke();
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

        if (bgmClip)
            BGMPlayer.ins.BlendNewBGM(bgmClip);

        _waveStarted = false;
        waves[_waveIndex].StartWave(spawnedSlimes, OnWaveStarted);
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

        entranceDetect.gameObject.SetActive(true);
        borderWall.SetActive(false);

        if (bgmClip)
            BGMPlayer.ins.ResetToBaseBGM();

        spawnedSlimes?.DestroyAll();

        _waveIndex = 0;
        foreach (FightWave wave in waves)
            wave.ResetFight();
    }
}
