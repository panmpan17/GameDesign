using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClipSet audioClipSet;

    [SerializeField]
    private RangeReference timerRange;
    private Timer _timer;

    void OnEnable()
    {
        _timer.TargetTime = timerRange.PickRandomNumber();
    }

    void OnDisable()
    {
        audioSource.Stop();
    }

    void Update()
    {
        if (_timer.UpdateEnd)
        {
            _timer.Reset();
            _timer.TargetTime = timerRange.PickRandomNumber();

            audioSource.Play(audioClipSet);
        }
    }
}
