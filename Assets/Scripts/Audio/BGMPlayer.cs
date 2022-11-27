using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer ins;

    [SerializeField]
    private BGMClip baseBGMClip;
    [SerializeField]
    private AudioSource track1;
    [SerializeField]
    private AudioSource track2;

    void Awake()
    {
        if (ins != null)
        {
            ins.BlendNewBGM(baseBGMClip);
            Destroy(gameObject);
            return;
        }
        ins = this;
        DontDestroyOnLoad(gameObject);
        PlayBGM(baseBGMClip, overrideCurrentBGM: true);
    }



    public void PlayBGM(BGMClip bgmClip, bool overrideCurrentBGM = false)
    {
        if (track1.isPlaying)
        {
            if (!overrideCurrentBGM) return;
            track1.Stop();
            track1.clip = null;
        }
        if (track2.isPlaying)
        {
            if (!overrideCurrentBGM) return;
            track2.Stop();
            track2.clip = null;
        }

        track1.clip = bgmClip.Clip;
        track1.volume = baseBGMClip.VolumeLevel;
        track1.Play();
    }

    public void BlendNewBGM(BGMClip bgmClip)
    {
        if (!track1.isPlaying && !track2.isPlaying)
        {
            PlayBGM(bgmClip);
            return;
        }
        // if (track1.isPlaying && track2.isPlaying)
        // TODO: Handle if two bgm audio source both playing

        if (track1.isPlaying)
        {
            track2.clip = bgmClip.Clip;
            StartCoroutine(FadeAudioSource(track1, 0, bgmClip.FadeOutTime, bgmClip.FadeOutCurve.Value, stopAfterFade: true));
            track2.volume = 0;
            StartCoroutine(FadeAudioSource(track2, track1.volume, bgmClip.FadeInTime, bgmClip.FadeInCurve.Value, playerAfterDelay: true, returnVolume: false));
        }
        else
        {
            track1.clip = bgmClip.Clip;
            StartCoroutine(FadeAudioSource(track2, 0, bgmClip.FadeOutTime, bgmClip.FadeOutCurve.Value, stopAfterFade: true));
            track1.volume = 0;
            StartCoroutine(FadeAudioSource(track1, track2.volume, bgmClip.FadeInTime, bgmClip.FadeInCurve.Value, playerAfterDelay: true, returnVolume: false));
        }
    }

    public void ResetToBaseBGM()
    {
        BlendNewBGM(baseBGMClip);
    }


    public IEnumerator FadeAudioSource(
        AudioSource src, float targetVolume, float fadeTime, AnimationCurve volumeCurve,
        bool returnVolume=true, bool playerAfterDelay=false, bool stopAfterFade=false)
    {
        if (playerAfterDelay) src.Play();

        float time = 0;
        float originVolume = src.volume;

        while (time < fadeTime)
        {
            yield return null;
            time += Time.deltaTime;
            src.volume = Mathf.Lerp(originVolume, targetVolume, volumeCurve.Evaluate(time / fadeTime));
        }

        if (stopAfterFade) src.Stop();
        if (returnVolume) src.volume = originVolume;
        else src.volume = targetVolume;
    }
}
