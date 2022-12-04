using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBarControl : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer fillSpriteRenderer;
    [SerializeField]
    private Image image;
    private RectTransform rectTransform;
    private bool _initialize;
    [SerializeField]
    private BarType barType;

    private Vector2 _originSize;
    private float _amount;

    public float Amount => _amount;

    void Awake()
    {
        if (_initialize)
            return;

        _initialize = true;
        if (fillSpriteRenderer)
            _originSize = fillSpriteRenderer.size;
        if (image)
        {}
        else
        {
            rectTransform = GetComponent<RectTransform>();
            _originSize = rectTransform.sizeDelta;
        }
    }

    /// <summary>
    /// Set the fill of the bar
    /// </summary>
    /// <param name="amount">0 is 0%, 1 is 100%</param>
    public void SetFillAmount(float amount)
    {
        if (!_initialize)
            Awake();

        _amount = amount;
        
        if (image)
        {
            image.fillAmount = amount;
            return;
        }

        Vector2 newSize = _originSize;
        if (barType == BarType.Horizontal)
        {
            newSize.x *= amount;
        }
        else
        {
            newSize.y *= amount;
        }

        if (fillSpriteRenderer != null)
            fillSpriteRenderer.size = newSize;
        else
            rectTransform.sizeDelta = newSize;
    }

    private enum BarType
    {
        Horizontal,
        Vertical
    }
}
