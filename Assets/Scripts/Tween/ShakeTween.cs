using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class ShakeTween : MonoBehaviour
{
    [SerializeField]
    private Timer duration;
    [SerializeField]
    private Timer interval;
    [SerializeField]
    private Vector3 shakeSize;
    [SerializeField]
    private bool anchorMode;

    private RectTransform _rectTransform;
    private Vector2 _originAnchoredPosition;
    private Vector3 _originPosition;

    void Awake()
    {
        if (anchorMode)
        {
            _rectTransform = GetComponent<RectTransform>();
            _originAnchoredPosition = _rectTransform.anchoredPosition;
        }
        else
        {
            _originPosition = transform.position;
        }
    }

    void Update()
    {
        if (duration.UpdateEnd)
        {
            enabled = false;

            if (anchorMode) _rectTransform.anchoredPosition = _originAnchoredPosition;
            else transform.position = _originPosition;
            return;
        }

        Vector3 delta = new Vector3(
            Random.Range(-shakeSize.x, shakeSize.x),
            Random.Range(-shakeSize.y, shakeSize.y),
            Random.Range(-shakeSize.z, shakeSize.z));

        if (anchorMode) _rectTransform.anchoredPosition = _originAnchoredPosition + (Vector2)delta;
        else transform.position = _originPosition + delta;
    }

    public void Trigger()
    {
        enabled = true;
        duration.Reset();
        interval.Reset();
    }
}
