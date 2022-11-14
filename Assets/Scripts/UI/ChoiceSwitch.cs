using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DigitalRuby.Tween;

public class ChoiceSwitch : Selectable
{
    [SerializeField]
    private bool hideLeftRight = true;
    [SerializeField]
    private Image left;
    private Vector2 leftAnchorPosition;
    [SerializeField]
    private Image right;
    private Vector2 rightAnchorPosition;
    [SerializeField]
    private Vector2 anchorOffset;
    [SerializeField]
    private float tweenTime;

    public UnityEvent leftEvent;
    public UnityEvent rightEvent;

    protected override void Start()
    {
        base.Start();

        if (hideLeftRight)
        {
            left.color = new Color(left.color.r, left.color.g, left.color.b, 0);
            right.color = new Color(right.color.r, right.color.g, right.color.b, 0);
        }
        else
        {
            left.color = new Color(left.color.r, left.color.g, left.color.b, 1);
            right.color = new Color(right.color.r, right.color.g, right.color.b, 1);
        }

        leftAnchorPosition = left.rectTransform.anchoredPosition;
        rightAnchorPosition = right.rectTransform.anchoredPosition;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.lastPress != eventData.selectedObject && hideLeftRight)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (eventData.position.x > transform.position.x)
            SwitchRight();
        else
            SwitchLeft();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        if (hideLeftRight)
        {
            FloatTween tween = gameObject.Tween("OnSelect", 0, 1, tweenTime, TweenScaleFunctions.CubicEaseIn, ChangeArrowAlpha);
            tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);

        if (hideLeftRight)
        {
            FloatTween tween = gameObject.Tween("OnDeselect", 1, 0, tweenTime, TweenScaleFunctions.CubicEaseIn, ChangeArrowAlpha);
            tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
        }
    }

    public override void OnMove(AxisEventData eventData)
    {
        switch (eventData.moveDir)
        {
            case MoveDirection.Left:
                SwitchLeft();
                break;

            case MoveDirection.Right:
                SwitchRight();
                break;

            case MoveDirection.Up:
            case MoveDirection.Down:
                base.OnMove(eventData);
                break;
        }
    }

    public void SwitchRight()
    {
        FloatTween tween = gameObject.Tween("Right", 0, 1, tweenTime, TweenScaleFunctions.QuarticEaseOut, ChangeRightArrowPosition, ResetRightArrowPosition);
        tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
        rightEvent.Invoke();
    }

    public void SwitchLeft()
    {
        FloatTween tween = gameObject.Tween("Left", 0, 1, tweenTime, TweenScaleFunctions.QuarticEaseOut, ChangeLeftArrowPosition, ResetLeftArrowPosition);
        tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
        leftEvent.Invoke();
    }

    void ChangeArrowAlpha(ITween<float> eventData)
    {
        Color color = left.color;
        color.a = eventData.CurrentValue;
        left.color = color;

        color = right.color;
        color.a = eventData.CurrentValue;
        right.color = color;
    }

    void ChangeLeftArrowPosition(ITween<float> eventData)
    {
        left.rectTransform.anchoredPosition = Vector2.Lerp(leftAnchorPosition, leftAnchorPosition - anchorOffset, eventData.CurrentValue);
    }

    void ResetLeftArrowPosition(ITween<float> eventData)
    {
        left.rectTransform.anchoredPosition = leftAnchorPosition;
    }

    void ChangeRightArrowPosition(ITween<float> eventData)
    {
        right.rectTransform.anchoredPosition = Vector2.Lerp(rightAnchorPosition, rightAnchorPosition + anchorOffset, eventData.CurrentValue);
    }

    void ResetRightArrowPosition(ITween<float> eventData)
    {
        right.rectTransform.anchoredPosition = rightAnchorPosition;
    }
}
