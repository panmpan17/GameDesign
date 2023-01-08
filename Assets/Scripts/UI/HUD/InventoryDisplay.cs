using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DigitalRuby.Tween;

public class InventoryDisplay : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private float countTextJumpTime = 0.28f;
    [SerializeField]
    private Vector2 countTextJumpDelta;
    [SerializeField]
    private Color countTextJumpingColor;

    [Space(5)]
    [SerializeField]
    private RectTransform coreCountRectTransform;
    [SerializeField]
    private TextMeshProUGUI coreCountText;
    private int _coreCount;
    private Vector2 _coreCountAnchoredPosition;

    [Space(5)]
    [SerializeField]
    private RectTransform appleCountRectTransform;
    [SerializeField]
    private TextMeshProUGUI appleCountText;
    private int _appleCount;
    private Vector2 _appleCountAnchoredPosition;

    [Space(5)]
    [SerializeField]
    private GameObject hasFlowerIcon;


    void Awake()
    {
        _coreCountAnchoredPosition = coreCountRectTransform.anchoredPosition;
        _appleCountAnchoredPosition = appleCountRectTransform.anchoredPosition;

        inventory.CoreEvent.InvokeIntEvents += ChangeCoreCount;
        inventory.AppleEvent.InvokeIntEvents += ChangeAppleCount;
        inventory.FlowerEvent.InvokeBoolEvents += ChangeHaveFlower;

        hasFlowerIcon.SetActive(false);
    }


    void OnDestroy()
    {
        inventory.CoreEvent.InvokeIntEvents -= ChangeCoreCount;
        inventory.AppleEvent.InvokeIntEvents -= ChangeAppleCount;
        inventory.FlowerEvent.InvokeBoolEvents -= ChangeHaveFlower;
    }

    void ChangeCoreCount(int count)
    {
        if (count > _coreCount)
        {
            coreCountText.color = countTextJumpingColor;
            gameObject.Tween(
                "CoreCountTextJump",
                _coreCountAnchoredPosition, _coreCountAnchoredPosition + countTextJumpDelta,
                countTextJumpTime, TweenScaleFunctions.QuarticEaseOut,
                (tweenData) => coreCountRectTransform.anchoredPosition = tweenData.CurrentValue,
                (tweenData) =>
                {
                    coreCountRectTransform.anchoredPosition = _coreCountAnchoredPosition;
                    coreCountText.color = Color.white;
                });
        }

        _coreCount = count;
        coreCountText.text = count.ToString();
    }

    void ChangeAppleCount(int count)
    {
        if (count > _appleCount)
        {
            appleCountText.color = countTextJumpingColor;
            gameObject.Tween(
                "AppleCountTextJump",
                _appleCountAnchoredPosition, _appleCountAnchoredPosition + countTextJumpDelta,
                countTextJumpTime, TweenScaleFunctions.QuarticEaseOut,
                (tweenData) => appleCountRectTransform.anchoredPosition = tweenData.CurrentValue,
                (tweenData) =>
                {
                    appleCountRectTransform.anchoredPosition = _appleCountAnchoredPosition;
                    appleCountText.color = Color.white;
                });
        }

        _appleCount = count;
        appleCountText.text = count.ToString();
    }

    void ChangeHaveFlower(bool hasFlower)
    {
        hasFlowerIcon.SetActive(hasFlower);
    }
}
