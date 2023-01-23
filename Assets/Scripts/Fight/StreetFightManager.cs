using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MPack;

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

    [SerializeField]
    private EventReference fightStartedEvent;
    [SerializeField]
    private EventReference fightWaveUpdateEvent;
    [SerializeField]
    private EventReference fightEndedEvent;

    [UnityEngine.Serialization.FormerlySerializedAs("OnEndEvent")]
    public UnityEvent OnCompletedEvent;

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
            OnFinished();
            return;
        }

        if (waves[_waveIndex].CanStart(allSlimesAreDead))
        {
            waves[_waveIndex].StartWave(spawnedSlimes, OnWaveStarted);
            _waveStarted = false;
            _waveIndex++;
            fightWaveUpdateEvent?.Invoke(_waveIndex);
        }
    }

    void OnPlayerEnterEntrance()
    {
        playerReviveEvent.InvokeEvents += ResetFight;

        entranceDetect.gameObject.SetActive(false);
        borderWall.SetActive(true);

        if (bgmClip)
            BGMPlayer.ins.BlendNewBGM(bgmClip);

        AudioVolumeControl.ins.FadeOutEnvironmentVolume();

        _waveStarted = false;
        waves[_waveIndex].StartWave(spawnedSlimes, OnWaveStarted);
        _waveIndex++;

        enabled = true;

        GameManager.ins.StartFight();
        fightStartedEvent?.Invoke(waves.Length);
    }

    void OnWaveStarted()
    {
        _waveStarted = true;
    }


    void OnFinished()
    {
        enabled = false;

        borderWall.SetActive(false);

        if (bgmClip)
            BGMPlayer.ins.ResetToBaseBGM();

        AudioVolumeControl.ins.FadeInEnvironmentVolume();

        GameManager.ins.EndFight();
        fightEndedEvent?.Invoke();
        OnCompletedEvent.Invoke();
    }

    void ResetFight()
    {
        enabled = false;

        playerReviveEvent.InvokeEvents -= ResetFight;

        entranceDetect.gameObject.SetActive(true);
        borderWall.SetActive(false);

        if (bgmClip)
            BGMPlayer.ins.ResetToBaseBGM();

        AudioVolumeControl.ins.FadeInEnvironmentVolume();

        spawnedSlimes?.DestroyAll();

        _waveIndex = 0;
        foreach (FightWave wave in waves)
            wave.ResetFight();

        fightEndedEvent?.Invoke();
        GameManager.ins.EndFight();
    }
}
