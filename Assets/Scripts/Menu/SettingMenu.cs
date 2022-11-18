using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MPack;
using TMPro;

public class SettingMenu : AbstractMenu
{
    private const string MainVolume = "MainVolume";
    private const string MusicVolume = "MusicVolume";
    private const string SFXVolume = "SFXVolume";

    [SerializeField]
    private GameObject firstSelected;

    [Header("Control")]
    [SerializeField]
    private FloatVariable xSensity;
    [SerializeField]
    private FloatVariable ySensity;
    [SerializeField]
    private Slider xSensitySlider;
    [SerializeField]
    private Slider ySensitySlider;
    [SerializeField]
    private Toggle xSensityInevert;
    [SerializeField]
    private Toggle ySensityInevert;

    [Header("Audio")]
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private Slider mainVolumeSlider;
    [SerializeField]
    private Slider musicVolumeSlider;
    [SerializeField]
    private Slider sfxVolumeSlider;

    [SerializeField]
    private AnimationCurveReference volumeCurve;
    [SerializeField]
    private AnimationCurveReference inverseVolumeCurve;
    [SerializeField]
    private RangeReference volumeRange;

    private float _mainVolume;
    private float _musicVolume;
    private float _sfxVolume;

    [Header("Display")]
    [SerializeField]
    private ChoiceSwitch languageSwitch;

    [Space(8)]
    [SerializeField]
    private Vector2Int[] avalibleResolution;
    private int currentResolutionIndex;
    [SerializeField]
    private ChoiceSwitch resolutionSwitch;
    [SerializeField]
    private TextMeshProUGUI resolutionText;

    [Space(8)]
    [SerializeField]
    private ChoiceSwitch windowTypeSwitch;
    [SerializeField]
    private LanguageText windowTypeText;
    [SerializeField]
    [LauguageID]
    private int fullscreenLanguageID;
    [SerializeField]
    [LauguageID]
    private int windowedLanguageID;

    [Space(8)]
    [SerializeField]
    private ChoiceSwitch qualitySwitch;
    [SerializeField]
    private LanguageText qualityValueText;
    [SerializeField]
    [LauguageID]
    private int lowQualityLanguageID;
    [SerializeField]
    [LauguageID]
    private int mediumQualityLanguageID;
    [SerializeField]
    [LauguageID]
    private int highQualityLanguageID;


    private Canvas _canvas;

    void Awake()
    {
        BindUI();

        RegisterMenu();
        _canvas = GetComponent<Canvas>();
        enabled = _canvas.enabled = false;
    }

    void BindUI()
    {
        mainVolumeSlider.onValueChanged.AddListener(OnMainVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMuscVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        languageSwitch.leftEvent.AddListener(OnLanguageLeft);
        languageSwitch.rightEvent.AddListener(OnLanguageRight);

        resolutionSwitch.leftEvent.AddListener(DownSizeResolution);
        resolutionSwitch.rightEvent.AddListener(UpSizeResolution);

        windowTypeSwitch.leftEvent.AddListener(ToggleFullscreen);
        windowTypeSwitch.rightEvent.AddListener(ToggleFullscreen);

        qualitySwitch.leftEvent.AddListener(DecreaseQuality);
        qualitySwitch.rightEvent.AddListener(IncreaseQuality);
    }

    protected override void OpenMenu()
    {
        base.OpenMenu();

        ApplyCurrentSettingToUI();

        EventSystem.current.SetSelectedGameObject(firstSelected);
        enabled = _canvas.enabled = true;
    }

    void ApplyCurrentSettingToUI()
    {
        xSensitySlider.SetValueWithoutNotify(Mathf.Abs(xSensity.Value));
        xSensityInevert.SetIsOnWithoutNotify(xSensity.Value < 0);
        ySensitySlider.SetValueWithoutNotify(Mathf.Abs(ySensity.Value));
        ySensityInevert.SetIsOnWithoutNotify(ySensity.Value < 0);


        if (audioMixer.GetFloat(MainVolume, out float mainVolume))
        {
            mainVolumeSlider.SetValueWithoutNotify(
                inverseVolumeCurve.Value.Evaluate(volumeRange.InverseLerp(mainVolume)));
            _mainVolume = mainVolume;
        }
        if (audioMixer.GetFloat(MusicVolume, out float musicVolume))
        {
            musicVolumeSlider.SetValueWithoutNotify(
                inverseVolumeCurve.Value.Evaluate(volumeRange.InverseLerp(musicVolume)));
            _musicVolume = musicVolume;
        }
        if (audioMixer.GetFloat(SFXVolume, out float sfxVolume))
        {
            sfxVolumeSlider.SetValueWithoutNotify(
                inverseVolumeCurve.Value.Evaluate(volumeRange.InverseLerp(sfxVolume)));
            _sfxVolume = sfxVolume;
        }


        float width = Screen.width;
        float height = Screen.height;
        resolutionText.text = string.Format("{0} x {1}", width, height);
        for (int i = 0; i < avalibleResolution.Length; i++)
        {
            if (width >= avalibleResolution[i].x)
                currentResolutionIndex = i;
        }

        ApplyQualityText();
    }


#region Audio
    void OnMainVolumeChanged(float volumePercentage)
    {
        audioMixer.SetFloat(MainVolume, volumeRange.Lerp(volumeCurve.Value.Evaluate(volumePercentage)));
    }
    void OnMuscVolumeChanged(float volumePercentage)
    {
        audioMixer.SetFloat(MusicVolume, volumeRange.Lerp(volumeCurve.Value.Evaluate(volumePercentage)));
    }
    void OnSFXVolumeChanged(float volumePercentage)
    {
        audioMixer.SetFloat(SFXVolume, volumeRange.Lerp(volumeCurve.Value.Evaluate(volumePercentage)));
    }
#endregion


#region Language
    void OnLanguageLeft()
    {
        LanguageAssign.ins.PreviousLanguage();
    }
    void OnLanguageRight()
    {
        LanguageAssign.ins.NextLanguage();
    }
#endregion


#region Resolution & Display
    void DownSizeResolution()
    {
        if (currentResolutionIndex <= 0)
            return;

        Vector2Int resolution = avalibleResolution[--currentResolutionIndex];
        resolutionText.text = string.Format("{0} x {1}", resolution.x, resolution.y);
        Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
    }
    void UpSizeResolution()
    {
        if (currentResolutionIndex >= avalibleResolution.Length - 1)
            return;

        Vector2Int resolution = avalibleResolution[++currentResolutionIndex];
        resolutionText.text = string.Format("{0} x {1}", resolution.x, resolution.y);
        Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
    }


    void ToggleFullscreen()
    {
        if (Screen.fullScreen)
        {
            windowTypeText.ChangeId(windowedLanguageID);
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.Windowed);
        }
        else
        {
            windowTypeText.ChangeId(fullscreenLanguageID);
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
        }
    }
#endregion


#region Quality
    void DecreaseQuality()
    {
        QualitySettings.DecreaseLevel();
        ApplyQualityText();
    }
    void IncreaseQuality()
    {
        QualitySettings.IncreaseLevel();
        ApplyQualityText();
    }

    void ApplyQualityText()
    {
        int qualityLevel = QualitySettings.GetQualityLevel();
        switch (qualityLevel)
        {
            case 0:
                qualityValueText.ChangeId(lowQualityLanguageID);
                break;
            case 1:
                qualityValueText.ChangeId(mediumQualityLanguageID);
                break;
            case 2:
                qualityValueText.ChangeId(highQualityLanguageID);
                break;
        }
    }
#endregion


    public void Cancel()
    {
        audioMixer.SetFloat(MainVolume, _mainVolume);
        audioMixer.SetFloat(MusicVolume, _musicVolume);
        audioMixer.SetFloat(SFXVolume, _sfxVolume);
        enabled = _canvas.enabled = false;
        CloseMenu();
    }

    public void Save()
    {
        xSensity.Value = xSensityInevert.isOn ? -xSensitySlider.value : xSensitySlider.value;
        ySensity.Value = ySensityInevert.isOn ? -ySensitySlider.value : ySensitySlider.value;

        enabled = _canvas.enabled = false;
        CloseMenu();
    }
}
