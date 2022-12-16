using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DigitalRuby.Tween;
using MPack;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

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

    private Stopwatch _changeResolutionColddown;

    private FloatTween _fadeTween;

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
            if (_fadeTween != null) CancelFadePreviousTween();
            _fadeTween = gameObject.Tween(gameObject.name + "OnSelect", 0, 1, tweenTime, TweenScaleFunctions.CubicEaseIn, ChangeArrowAlpha, delegate { _fadeTween = null; });
            _fadeTween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);

        if (hideLeftRight)
        {
            if (_fadeTween != null) CancelFadePreviousTween();
            _fadeTween = gameObject.Tween(gameObject.name + "OnDeselect", 1, 0, tweenTime, TweenScaleFunctions.CubicEaseIn, ChangeArrowAlpha, delegate { _fadeTween = null; });
            _fadeTween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
        }
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (_changeResolutionColddown.DeltaTime < 0.4f)
            return;

        _changeResolutionColddown.Update();
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
        if (_fadeTween != null) CancelFadePreviousTween();
        _fadeTween = gameObject.Tween(gameObject.name + "Right", 0, 1, tweenTime, TweenScaleFunctions.QuarticEaseOut, ChangeRightArrowPosition, ResetRightArrowPosition);
        _fadeTween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
        rightEvent.Invoke();
    }

    public void SwitchLeft()
    {
        if (_fadeTween != null) CancelFadePreviousTween();
        _fadeTween = gameObject.Tween(gameObject.name + "Left", 0, 1, tweenTime, TweenScaleFunctions.QuarticEaseOut, ChangeLeftArrowPosition, ResetLeftArrowPosition);
        _fadeTween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
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
        _fadeTween = null;
    }

    void ResetRightArrowPosition(ITween<float> eventData)
    {
        right.rectTransform.anchoredPosition = rightAnchorPosition;
        _fadeTween = null;
    }

    void CancelFadePreviousTween()
    {
        _fadeTween.Stop(TweenStopBehavior.Complete);
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(ChoiceSwitch))]
public class ChoiceSwitchEditor : SelectableEditor
{
    private SerializedProperty hideLeftRight, left, right, anchorOffset, tweenTime;

    protected override void OnEnable()
    {
        base.OnEnable();

        hideLeftRight = serializedObject.FindProperty("hideLeftRight");
        left = serializedObject.FindProperty("left");
        right = serializedObject.FindProperty("right");
        anchorOffset = serializedObject.FindProperty("anchorOffset");
        tweenTime = serializedObject.FindProperty("tweenTime");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(hideLeftRight);
        EditorGUILayout.PropertyField(left);
        EditorGUILayout.PropertyField(right);
        EditorGUILayout.PropertyField(anchorOffset);
        EditorGUILayout.PropertyField(tweenTime);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif