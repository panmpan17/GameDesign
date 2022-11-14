using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MPack;
using TMPro;

public class SettingMenu : MonoBehaviour
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

    private float _originXSensity;
    private bool _originXSensityInvert;
    private float _originySensity;
    private bool _originySensityInvert;

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

    void Awake()
    {
        // xSensitySlider.onValueChanged.AddListener(XSensityChanged);
        // ySensitySlider.onValueChanged.AddListener(YSensityChanged);
        // xSensityInevert.onValueChanged.AddListener(XSensityInvertChanged);
        // ySensityInevert.onValueChanged.AddListener(YSensityInvertChanged);
        languageSwitch.leftEvent.AddListener(OnLanguageLeft);
        languageSwitch.rightEvent.AddListener(OnLanguageRight);

        resolutionSwitch.leftEvent.AddListener(DownSizeResolution);
        resolutionSwitch.rightEvent.AddListener(UpSizeResolution);

        windowTypeSwitch.leftEvent.AddListener(ToggleFullscreen);
        windowTypeSwitch.rightEvent.AddListener(ToggleFullscreen);

        float width = Screen.width;
        float height = Screen.height;
        resolutionText.text = string.Format("{0} x {1}", width, height);
        for (int i = 0; i < avalibleResolution.Length; i++)
        {
            if (width >= avalibleResolution[i].x)
                currentResolutionIndex = i;
        }


        qualitySwitch.leftEvent.AddListener(DecreaseQuality);
        qualitySwitch.rightEvent.AddListener(IncreaseQuality);
        ApplyQualityText();

        Activate();
    }

    public void Activate()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);

        _originXSensity = xSensitySlider.value;
        _originXSensityInvert = xSensityInevert.isOn;
        _originySensity = ySensitySlider.value;
        _originySensityInvert = ySensityInevert.isOn;

        gameObject.SetActive(true);
    }

    // void XSensityChanged(float value) {}
    // void YSensityChanged(float value) {}
    // void XSensityInvertChanged(bool value) {}
    // void YSensityInvertChanged(bool value) {}


    void OnLanguageLeft()
    {
        LanguageAssign.ins.PreviousLanguage();
    }
    void OnLanguageRight()
    {
        LanguageAssign.ins.NextLanguage();
    }


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


    public void Cancel()
    {
        xSensitySlider.value = _originXSensity;
        xSensityInevert.isOn = _originXSensityInvert;
        ySensitySlider.value = _originySensity;
        ySensityInevert.isOn = _originySensityInvert;

        gameObject.SetActive(false);
    }

    public void Save()
    {
        xSensity.Value = xSensityInevert.isOn ? -xSensitySlider.value : xSensitySlider.value;
        ySensity.Value = ySensityInevert.isOn ? -ySensitySlider.value : ySensitySlider.value;

        gameObject.SetActive(false);
    }
}
