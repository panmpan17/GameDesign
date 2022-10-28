using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MPack;

public class SettingMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject firstSelected;
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

    void Awake()
    {
        // xSensitySlider.onValueChanged.AddListener(XSensityChanged);
        // ySensitySlider.onValueChanged.AddListener(YSensityChanged);
        // xSensityInevert.onValueChanged.AddListener(XSensityInvertChanged);
        // ySensityInevert.onValueChanged.AddListener(YSensityInvertChanged);
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
