using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimCursor : MonoBehaviour
{
    [Header("Component Reference")]
    [SerializeField]
    private CanvasGroup aimProgressFrame;
    [SerializeField]
    private Image fill;
    [SerializeField]
    private EventReference aimProgressEvent;
    [SerializeField]
    private Image firstAim;
    [SerializeField]
    private Image secondAim;

    [Header("Alpha")]
    [SerializeField]
    private float aimFrameUnfocusAlpha;
    [SerializeField]
    private float aimFrameFocusAlpha;

    [Header("Color Lerp")]
    [SerializeField]
    private Color color1;
    [SerializeField]
    private Color color2;
    [SerializeField]
    private float min;
    [SerializeField]
    private float max;
    [SerializeField]
    private AnimationCurve curve;
    private float _firstAimPercentage;

    private MaterialPropertyBlock materialPropertyBlock;


    void Awake()
    {
        aimProgressFrame.alpha = aimFrameUnfocusAlpha;
        aimProgressEvent.InvokeFloatEvents += ChangeAimProgress;
        fill.fillAmount = 0;

        materialPropertyBlock = new MaterialPropertyBlock();
    }


    void ChangeAimProgress(float progress)
    {
        if (progress <= 0)
        {
            aimProgressFrame.alpha = aimFrameUnfocusAlpha;
            fill.fillAmount = 0;
            return;
        }

        aimProgressFrame.alpha = aimFrameFocusAlpha;

        if (progress <= 1)
        {
            fill.fillAmount = Mathf.Lerp(0, _firstAimPercentage, progress);
            fill.color = firstAim.color;
        }
        else
        {
            float progressMinus1 = progress - 1;
            fill.fillAmount = Mathf.Lerp(_firstAimPercentage, 1, progressMinus1);
            fill.color = Color.Lerp(color1, color2, progressMinus1);
        }
    }

    public void SetAimDuration(float firstAimDuration, float secondAimDuration)
    {
        float total = firstAimDuration + secondAimDuration;
        _firstAimPercentage = firstAimDuration / total;
        float secondPercentage = secondAimDuration / total;
        firstAim.fillAmount = _firstAimPercentage;
        secondAim.fillAmount = secondPercentage;
        secondAim.transform.rotation = Quaternion.Euler(0, 0, _firstAimPercentage * -360);

        secondAim.material.SetFloat("_Degree", Mathf.Lerp(min, max, curve.Evaluate(secondPercentage)));
    }

    void OnDestroy()
    {
        aimProgressEvent.InvokeFloatEvents -= ChangeAimProgress;
    }
}
