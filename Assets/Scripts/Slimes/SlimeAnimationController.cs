using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using XnodeBehaviourTree;


public class SlimeAnimationController : MonoBehaviour, IAnimationPlayer
{
    [SerializeField]
    private SpriteRenderer faceSpriteRenderer;
    [SerializeField]
    private SpriteAnimation[] spriteAnimations;
    [SerializeField]
    private ValueWithEnable<int> startAnimationIndex;
    [SerializeField]
    private int[] randomPlayedIndex;

    private int _animationIndex;
    private int _ainmationKeyIndex;
    private Timer _timer;

    private string _tempDestinateAnimation;

    void Start()
    {
        if (!startAnimationIndex.Enable)
        {
            enabled = false;
            return;
        }

        SwitchToAnimation(startAnimationIndex.Value);
    }

    void Update()
    {
        if (_timer.UpdateEnd)
        {
            _timer.Reset();

            SpriteAnimation animation = spriteAnimations[_animationIndex];
            if (++_ainmationKeyIndex >= animation.KeyPoints.Length)
            {
                HandleAnimationEnd(animation);
                return;
            }

            faceSpriteRenderer.sprite = animation.KeyPoints[_ainmationKeyIndex].Sprite;
            _timer.TargetTime = animation.KeyPoints[_ainmationKeyIndex].Interval;
        }
    }

    void HandleAnimationEnd(SpriteAnimation animation)
    {
        switch (animation.EndWrapMode)
        {
            case SpriteAnimation.WrapMode.Hold:
                enabled = false;
                return;

            case SpriteAnimation.WrapMode.LoopDestinateAnimations:
                if (_tempDestinateAnimation != "")
                {
                    SwitchToAnimation(_tempDestinateAnimation);
                    _tempDestinateAnimation = "";
                    return;
                }

                _animationIndex = randomPlayedIndex[Random.Range(0, randomPlayedIndex.Length)];
                animation = spriteAnimations[_animationIndex];
                break;
        }

        _ainmationKeyIndex = 0;
        faceSpriteRenderer.sprite = animation.KeyPoints[_ainmationKeyIndex].Sprite;
        _timer.TargetTime = animation.KeyPoints[_ainmationKeyIndex].Interval;
    }

    public void SwitchToAnimation(int index)
    {
        _animationIndex = index;
        _ainmationKeyIndex = 0;

        SpriteAnimation animation = spriteAnimations[_animationIndex];
        faceSpriteRenderer.sprite = animation.KeyPoints[0].Sprite;
        _timer.TargetTime = animation.KeyPoints[0].Interval;

        enabled = true;
    }
    public void SwitchToAnimation(string name)
    {
        for (int i = 0; i < spriteAnimations.Length; i++)
        {
            if (spriteAnimations[i].AnimatinName != name)
                continue;

            SwitchToAnimation(i);
            break;
        }
    }

    public void PlayAnimation(string animationName)
    {
        SwitchToAnimation(animationName);
    }

    public void SetOneTimeDestinateAnimation(string animationName)
    {
        _tempDestinateAnimation = animationName;
    }
}
