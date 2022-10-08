using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using MPack;

public class PlayerAnimation : MonoBehaviour
{
    private static readonly int AnimKeyWalking = Animator.StringToHash("Walking");
    private static readonly int AnimKeyJump = Animator.StringToHash("Jump");
    private static readonly int AnimKeyRejump = Animator.StringToHash("Rejump");
    private static readonly int AnimKeyEndJump = Animator.StringToHash("EndJump");
    private static readonly int AnimKeyDrawingBow = Animator.StringToHash("DrawingBow");
    private static readonly int AnimKeyWalkSpeed = Animator.StringToHash("WalkingSpeed");
    private static readonly int AnimKeyRoll = Animator.StringToHash("Roll");
    private static readonly int AnimKeyDeath = Animator.StringToHash("Death");

    private static readonly int AnimNameIdle = Animator.StringToHash("Idle");
    
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

    [Header("Others")]
    [SerializeField]
    private FloatReference drawBowSlowDown;

    [Header("Editor only")]
    [SerializeField]
    private LayerMask groundLayers;

    private bool _walking = false;

    private bool _drawBow = false;
    private Transform _prepareArrowTransform;

    private Coroutine _weightTweenRoutine;

    public bool IsDrawArrowFullyPlayed => animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1 && !animator.IsInTransition(1);
    public float RollAnimationProgress => animator.GetCurrentAnimatorStateInfo(0).normalizedTime;


    void Awake()
    {
        movement.OnJumpEvent += OnJump;
        movement.OnJumpEndEvent += OnJumpEnd;
        movement.OnRejumpEvent += OnRejump;
        movement.OnRollEvent += OnRoll;

        behaviour.OnDrawBow += OnDrawBow;
        behaviour.OnDrawBowEnd += OnDrawBowEnd;
        behaviour.OnDeath += OnDeath;
        behaviour.OnRevive += OnRevive;
    }

    void LateUpdate()
    {
        if (behaviour.IsDead)
            return;

        if (_drawBow)
        {
            RotateChest();
            // TestRotateChest();
        }


        if (movement.IsWalking != _walking)
        {
            _walking = movement.IsWalking;
            animator.SetBool(AnimKeyWalking, _walking);
        }
    }

    void OnJump()
    {
        animator.ResetTrigger(AnimKeyEndJump);
        animator.SetTrigger(AnimKeyJump);
    }

    void OnJumpEnd()
    {
        animator.ResetTrigger(AnimKeyJump);
        animator.SetTrigger(AnimKeyEndJump);
    }

    void OnRejump()
    {
        animator.ResetTrigger(AnimKeyJump);
        animator.ResetTrigger(AnimKeyEndJump);
        animator.SetTrigger(AnimKeyRejump);
    }

    void OnRoll()
    {
        animator.SetTrigger(AnimKeyRoll);
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


#region Player behaviour event
    void OnDrawBow()
    {
        _drawBow = true;
        _prepareArrowTransform = behaviour.PreparedArrow.transform;
        animator.SetBool(AnimKeyDrawingBow, true);
        animator.SetFloat(AnimKeyWalkSpeed, drawBowSlowDown.Value);

        if (_weightTweenRoutine != null)
            StopCoroutine(_weightTweenRoutine);
        _weightTweenRoutine = StartCoroutine(TweenRigWeight(0, 1, 0.2f));
    }

    void OnDrawBowEnd()
    {
        _drawBow = false;
        _prepareArrowTransform = null;
        animator.SetBool(AnimKeyDrawingBow, false);
        animator.SetFloat(AnimKeyWalkSpeed, 1);

        if (_weightTweenRoutine != null)
            StopCoroutine(_weightTweenRoutine);
        _weightTweenRoutine = StartCoroutine(TweenRigWeight(1, 0, 0.2f));
    }

    void OnDeath()
    {
        animator.ResetTrigger(AnimKeyJump);
        animator.ResetTrigger(AnimKeyRejump);
        animator.ResetTrigger(AnimKeyEndJump);
        animator.ResetTrigger(AnimKeyRoll);
        animator.SetBool(AnimKeyWalking, false);
        animator.SetBool(AnimKeyDrawingBow, false);

        animator.SetTrigger(AnimKeyDeath);
    }

    void OnRevive()
    {
        animator.Play(AnimNameIdle);
    }
#endregion


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
