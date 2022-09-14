using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;

public class PlayerAnimation : MonoBehaviour
{
    private const string AnimKeyWalking = "Walking";
    private const string AnimKeyJump = "Jump";
    private const string AnimKeyEndJump = "EndJump";
    private const string AnimKeyDrawingBow = "DrawingBow";
    private const string AnimKeyWalkSpeed = "WalkSpeed";
    
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private PlayerBehaviour behaviour;
    [SerializeField]
    private CinemachineBlenderSettings cameraBlendSettings;

    [Header("Rig reference")]
    [SerializeField]
    private Rig rig;
    [SerializeField]
    private Transform neck;
    [SerializeField]
    private Transform chest;
    [SerializeField]
    private Quaternion chestRotationOffsetA;
    [SerializeField]
    private Quaternion chestRotationOffsetB;
    [SerializeField]
    private Quaternion chestRotationOffset;

    private int _walkingKey = 0;
    private int _jumpKey = 0;
    private int _jumpEndKey = 0;
    private int _drawingBowKey = 0;
    private int _walkingSpeedKey = 0;

    private bool _walking = false;

    private bool _drawBow = false;

    private Coroutine _weightTweenRoutine;


    void Awake()
    {
        movement.OnJump += OnJump;
        movement.OnJumpEnd += OnJumpEnd;

        behaviour.OnDrawBow += OnDrawBow;
        behaviour.OnDrawBowEnd += OnDrawBowEnd;

        FindAnimatorKey();
    }

    void FindAnimatorKey()
    {
        _walkingKey = Animator.StringToHash(AnimKeyWalking);
        _jumpKey = Animator.StringToHash(AnimKeyJump);
        _jumpEndKey = Animator.StringToHash(AnimKeyEndJump);
        _drawingBowKey = Animator.StringToHash(AnimKeyDrawingBow);
        _walkingSpeedKey = Animator.StringToHash(AnimKeyWalkSpeed);
    }

    void LateUpdate()
    {
        if (_drawBow)
        {
            Quaternion chestRotation = Quaternion.LookRotation(behaviour.CurrentRayHitPosition - chest.position, transform.up);
            // chestRotationOffset = Quaternion.Lerp(chestRotationOffsetA, chestRotationOffsetB, movement.AngleLerpValue);
            // chest.rotation = chestRotation * Quaternion.Lerp(chestRotationOffsetA, chestRotationOffsetB, movement.AngleLerpValue);
            chest.rotation = chestRotation * chestRotationOffset;
        }


        if (movement.IsWalking != _walking)
        {
            _walking = movement.IsWalking;
            animator.SetBool(_walkingKey, _walking);
        }
    }

    void OnJump()
    {
        animator.ResetTrigger(_jumpEndKey);
        animator.SetTrigger(_jumpKey);
    }

    void OnJumpEnd()
    {
        animator.ResetTrigger(_jumpKey);
        animator.SetTrigger(_jumpEndKey);
    }


    void OnDrawBow()
    {
        _drawBow = true;
        animator.SetBool(_drawingBowKey, true);

        if (_weightTweenRoutine != null)
            StopCoroutine(_weightTweenRoutine);
        _weightTweenRoutine = StartCoroutine(TweenRigWeight(0, 1, 0.2f));
    }
    void OnDrawBowEnd()
    {
        _drawBow = false;
        animator.SetBool(_drawingBowKey, false);

        if (_weightTweenRoutine != null)
            StopCoroutine(_weightTweenRoutine);
        _weightTweenRoutine = StartCoroutine(TweenRigWeight(1, 0, 0.2f));
    }

    IEnumerator TweenRigWeight(float fromWeight, float toWeight, float duration)
    {
        float timePassed = Mathf.InverseLerp(fromWeight, toWeight, rig.weight) * duration;

        while (true)
        {
            yield return null;
            timePassed += Time.deltaTime;
            rig.weight = Mathf.Lerp(fromWeight, toWeight, timePassed / duration);

            if (timePassed >= duration)
                break;
        }

        _weightTweenRoutine = null;
    }
}
