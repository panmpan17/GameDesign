using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;
// using UnityEngine.InputSystem.Utilities;
using MPack;
using TMPro;

public class SettingMenu : AbstractMenu
{
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
    private Stopwatch _changeResolutionColddown;

    [Space(8)]
    [SerializeField]
    private Toggle isFullscreenToggle;

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


    private InputAction _cancelAction;
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
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        languageSwitch.leftEvent.AddListener(OnLanguageLeft);
        languageSwitch.rightEvent.AddListener(OnLanguageRight);

        resolutionSwitch.leftEvent.AddListener(DownSizeResolution);
        resolutionSwitch.rightEvent.AddListener(UpSizeResolution);

        isFullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);

        qualitySwitch.leftEvent.AddListener(DecreaseQuality);
        qualitySwitch.rightEvent.AddListener(IncreaseQuality);
    }

    protected override void OpenMenu()
    {
        base.OpenMenu();

        ApplyCurrentSettingToUI();

        EventSystem.current.SetSelectedGameObject(firstSelected);
        enabled = _canvas.enabled = true;

        if (_cancelAction == null)
            _cancelAction = EventSystem.current.GetComponent<InputSystemUIInputModule>().cancel.action;
        _cancelAction.performed += OnCancel;
    }

    void ApplyCurrentSettingToUI()
    {
        xSensitySlider.SetValueWithoutNotify(Mathf.Abs(xSensity.Value));
        xSensityInevert.SetIsOnWithoutNotify(xSensity.Value < 0);
        ySensitySlider.SetValueWithoutNotify(Mathf.Abs(ySensity.Value));
        ySensityInevert.SetIsOnWithoutNotify(ySensity.Value < 0);


        mainVolumeSlider.SetValueWithoutNotify(AudioVolumeControl.ins.MainVolumeSliderValue);
        musicVolumeSlider.SetValueWithoutNotify(AudioVolumeControl.ins.MusicVolumeSliderValue);
        sfxVolumeSlider.SetValueWithoutNotify(AudioVolumeControl.ins.SFXVolumeSliderValue);


        float width = Screen.width;
        float height = Screen.height;
        resolutionText.text = string.Format("{0} x {1}", width, height);
        for (int i = 0; i < avalibleResolution.Length; i++)
        {
            if (width >= avalibleResolution[i].x)
                currentResolutionIndex = i;
        }

        isFullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);

        ApplyQualityText();
    }


#region Audio
    void OnMainVolumeChanged(float volumePercentage)
    {
        AudioVolumeControl.ins.MainVolumeSliderValue = volumePercentage;
    }
    void OnMusicVolumeChanged(float volumePercentage)
    {
        AudioVolumeControl.ins.MusicVolumeSliderValue = volumePercentage;
    }
    void OnSFXVolumeChanged(float volumePercentage)
    {
        AudioVolumeControl.ins.SFXVolumeSliderValue = volumePercentage;
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

    void ToggleFullscreen(bool isFullscreen)
    {
        Screen.SetResolution(Screen.width, Screen.height, isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
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
        AudioVolumeControl.ins.ReloadSoundVolumeFromPreference();
        enabled = _canvas.enabled = false;

        _cancelAction.performed -= OnCancel;
        CloseMenu();
    }

    public void Save()
    {
        AudioVolumeControl.ins.SaveVolumeToPreference();

        xSensity.Value = xSensityInevert.isOn ? -xSensitySlider.value : xSensitySlider.value;
        ySensity.Value = ySensityInevert.isOn ? -ySensitySlider.value : ySensitySlider.value;

        enabled = _canvas.enabled = false;

        _cancelAction.performed -= OnCancel;
        CloseMenu();
    }


    void OnCancel(CallbackContext callbackContext)
    {
        Save();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_cancelAction != null)
            _cancelAction.performed -= OnCancel;
    }
}
