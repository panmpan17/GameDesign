using System.Collections;
using System.Collections.Generic;
using MPack;
using UnityEngine;
using UnityEngine.Audio;


public class AudioVolumeControl : MonoBehaviour
{
    private const string Mixer_MainVolume = "MainVolume";
    private const string Mixer_MusicVolume = "MusicVolume";
    private const string Mixer_SFXVolume = "SFXVolume";
    private const string Mixer_EnviromentVolume = "EnviromentVolume";

    private const string Prefs_MainVolume = "MainVolumeSliderValue";
    private const string Prefs_MusicVolume = "MusicVolumeSliderValue";
    private const string Prefs_SFXVolume = "SFXVolumeSliderValue";


    public static AudioVolumeControl ins;

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private RangeReference mainVolumeRange;
    [SerializeField]
    private RangeReference musicVolumeRange;
    [SerializeField]
    private RangeReference SFXVolumeRange;
    [SerializeField]
    private AnimationCurveReference volumeCurve;

    [SerializeField]
    private float environmentVolumeFadeOutVolume;
    [SerializeField]
    private Timer environmentVolumeFadeTimer;


#region Property
    public float MainVolumeSliderValue {
        get => _mainVolumeSliderValue;
        set {
            _mainVolumeSliderValue = value;
            audioMixer.SetFloat(Mixer_MainVolume, MainVolume);
        }
    }
    private float _mainVolumeSliderValue;

    public float MusicVolumeSliderValue {
        get => _musicVolumeSliderValue;
        set {
            _musicVolumeSliderValue = value;
            audioMixer.SetFloat(Mixer_MusicVolume, MusicVolume);
        }
    }
    private float _musicVolumeSliderValue;

    public float SFXVolumeSliderValue {
        get => _sfxVolumeSliderValue;
        set {
            _sfxVolumeSliderValue = value;
            audioMixer.SetFloat(Mixer_SFXVolume, SFXVolume);
        }
    }
    private float _sfxVolumeSliderValue;

    public float MainVolume => mainVolumeRange.Lerp(volumeCurve.Evaluate(_mainVolumeSliderValue));
    public float MusicVolume => musicVolumeRange.Lerp(volumeCurve.Evaluate(_musicVolumeSliderValue));
    public float SFXVolume => SFXVolumeRange.Lerp(volumeCurve.Evaluate(_sfxVolumeSliderValue));

    private float _originalEnviromentVolume;
#endregion


    void Awake()
    {
        if (ins)
        {
            Destroy(gameObject);
            return;
        }
        ins = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ReloadSoundVolumeFromPreference();
        if (audioMixer.GetFloat(Mixer_EnviromentVolume, out float value))
            _originalEnviromentVolume = value;
    }

    public void ReloadSoundVolumeFromPreference()
    {
        MainVolumeSliderValue = PlayerPrefs.GetFloat(Prefs_MainVolume, 1);
        MusicVolumeSliderValue = PlayerPrefs.GetFloat(Prefs_MusicVolume, 1);
        SFXVolumeSliderValue = PlayerPrefs.GetFloat(Prefs_SFXVolume, 1);
    }

    public void SaveVolumeToPreference()
    {
        PlayerPrefs.SetFloat(Prefs_MainVolume, MainVolumeSliderValue);
        PlayerPrefs.SetFloat(Prefs_MusicVolume, MusicVolumeSliderValue);
        PlayerPrefs.SetFloat(Prefs_SFXVolume, SFXVolumeSliderValue);
    }


    public void FadeInEnvironmentVolume()
    {
        audioMixer.SetFloat(Mixer_EnviromentVolume, _originalEnviromentVolume);
    }

    public void FadeOutEnvironmentVolume()
    {
        audioMixer.SetFloat(Mixer_EnviromentVolume, environmentVolumeFadeOutVolume);
    }
}
