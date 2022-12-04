using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;

public class ColorTween : MonoBehaviour
{
    [SerializeField]
    private Timer interval;
    [SerializeField]
    private AnimationCurveReference curveReference;
    [SerializeField]
    private Color color1;
    [SerializeField]
    private Color color2;

    private Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        if (interval.UpdateEnd)
            interval.ReverseMode = !interval.ReverseMode;

        _image.color = Color.Lerp(color1, color2, interval.Progress);
    }

    public void ResetTween(float progress)
    {
        interval.Reset();
        interval.ReverseMode = false;
        _image.color = Color.Lerp(color1, color2, progress);
    }
}
