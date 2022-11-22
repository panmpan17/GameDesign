using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class AnchorPositionTween : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private Vector2 startPosition;
    [SerializeField]
    private Vector2 endPosition;
    [SerializeField]
    private Timer timer;

    private RectTransform _rectTransform;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        _rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, curve.Evaluate(timer.Progress));
        if (timer.UpdateEnd)
        {
            timer.ReverseMode = !timer.ReverseMode;
        }
    }
}
