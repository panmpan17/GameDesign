using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapUI : MonoBehaviour
{
    [SerializeField]
    private EventReference enlargeEvent;

    [Header("Enlarge Mode")]
    [SerializeField]
    private Vector2 enlargeAnchor;
    [SerializeField]
    private Vector2 enlargeSizeDelta;
    [SerializeField]
    private Vector2 enlargeAnchoredPosition;

    private RectTransform _rectTransform;
    private Vector2 _smallAnchor;
    private Vector2 _smallSizeDelta;
    private Vector2 _smallAnchoredPosition;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _smallAnchor = _rectTransform.anchorMin;
        _smallSizeDelta = _rectTransform.sizeDelta;
        _smallAnchoredPosition = _rectTransform.anchoredPosition;

        enlargeEvent.InvokeBoolEvents += ChangeEnlargeMode;
    }

    void ChangeEnlargeMode(bool isEnlarge)
    {
        if (isEnlarge)
        {
            _rectTransform.anchorMin = enlargeAnchor;
            _rectTransform.anchorMax = enlargeAnchor;
            _rectTransform.anchoredPosition = enlargeAnchoredPosition;
            _rectTransform.sizeDelta = enlargeSizeDelta;
        }
        else
        {
            _rectTransform.anchorMin = _smallAnchor;
            _rectTransform.anchorMax = _smallAnchor;
            _rectTransform.anchoredPosition = _smallAnchoredPosition;
            _rectTransform.sizeDelta = _smallSizeDelta;
        }
    }
}
