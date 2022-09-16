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
    private const string AnimKeyRoll = "Roll";
    
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

    [SerializeField]
    private Transform drawBowLeftHandFinalPosition;
    [SerializeField]
    private Transform drawBowRightHandFinalPosition;
    
    [SerializeField]
    private Transform rightHand;
    [SerializeField]
    private Transform bow;
    [SerializeField]
    private Quaternion bowRotationOffset;

    [Header("Editor only")]
    [SerializeField]
    private LayerMask groundLayers;

    private int _walkingKey = 0;
    private int _jumpKey = 0;
    private int _jumpEndKey = 0;
    private int _drawingBowKey = 0;
    private int _walkingSpeedKey = 0;
    private int _rollKey = 0;

    private bool _walking = false;

    private bool _drawBow = false;
    private Transform _prepareArrowTransform;

    private Coroutine _weightTweenRoutine;

    public bool IsDrawArrowFullyPlayed => animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1 && !animator.IsInTransition(1);
    public float RollAnimationProgress => animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    // public bool IsRollFullyPlayed => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !animator.IsInTransition(1);


    void Awake()
    {
        movement.OnJump += OnJump;
        movement.OnJumpEnd += OnJumpEnd;
        movement.OnRoll += OnRoll;

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
        _rollKey = Animator.StringToHash(AnimKeyRoll);
    }

    void LateUpdate()
    {
        if (_drawBow)
        {
            RotateChest();
            // TestRotateChest();
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

    void OnRoll()
    {
        animator.SetTrigger(_rollKey);
    }

    void RotateChest()
    {
        Quaternion chestRotation = Quaternion.LookRotation(behaviour.CurrentRayHitPosition - chest.position, transform.up);
        chest.rotation = chestRotation * Quaternion.Lerp(chestRotationOffsetA, chestRotationOffsetB, movement.AngleLerpValue);

        bow.rotation = Quaternion.LookRotation(bow.position - rightHand.position, transform.up) * bowRotationOffset;
        _prepareArrowTransform.rotation = Quaternion.LookRotation(_prepareArrowTransform.position - rightHand.position, _prepareArrowTransform.up);
    }

    void TestRotateChest()
    {
        Vector3 hitPosition = behaviour.CurrentRayHitPosition;

        Quaternion chestRotation = Quaternion.LookRotation(hitPosition - chest.position, transform.up);
        chest.rotation = chestRotation * chestRotationOffset;

        Debug.DrawRay(hitPosition, Vector3.up * 3, Color.white, 0.1f);
        Debug.DrawLine(chest.position, hitPosition, Color.red, 0.1f);

        Vector3 arrowVector = drawBowLeftHandFinalPosition.position - drawBowRightHandFinalPosition.position;
        Debug.DrawRay(chest.position, arrowVector * 15, Color.yellow, 0.1f);
        Debug.DrawRay(drawBowRightHandFinalPosition.position, arrowVector * 15, Color.green, 0.1f);
    }

    void OnDrawBow()
    {
        _drawBow = true;
        _prepareArrowTransform = behaviour.PreparedArrow.transform;
        animator.SetBool(_drawingBowKey, true);

        if (_weightTweenRoutine != null)
            StopCoroutine(_weightTweenRoutine);
        _weightTweenRoutine = StartCoroutine(TweenRigWeight(0, 1, 0.2f));
    }
    void OnDrawBowEnd()
    {
        _drawBow = false;
        _prepareArrowTransform = null;
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