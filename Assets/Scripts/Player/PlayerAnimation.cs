using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
// using Cinemachine;
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
    private static readonly int AnimNameEmpty = Animator.StringToHash("Empty");

    private static readonly int DrawBowSpeed = Animator.StringToHash("DrawBowSpeed");
    private static readonly int RollSpeed = Animator.StringToHash("RollSpeed");
    
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private PlayerBehaviour behaviour;

    [Header("Rig reference")]
    [SerializeField]
    private Transform chest;
    [SerializeField]
    private Quaternion chestRotationOffsetA;
    [SerializeField]
    private Quaternion chestRotationOffsetB;

    [HideInInspector]
    [SerializeField]
    private Transform drawBowLeftHandFinalPosition, drawBowRightHandFinalPosition;

    [SerializeField]
    private Transform rightHand;
    [SerializeField]
    private Transform bow;
    [SerializeField]
    private Quaternion bowRotationOffset;

    [Header("Others")]
    [SerializeField]
    private FloatReference drawBowSlowDown;
    [SerializeField]
    private ParticleSystem stepDustParticle;
    [SerializeField]
    private TransformPointer currentArrowPointer;
    [SerializeField]
    private EffectReference levelUpEffect;

    [Header("Change Animation Speed")]
    [SerializeField]
    private AnimationClip drawBowClip;
    [SerializeField]
    private AnimationClip rollClip;

    [Header("Editor only")]
    [SerializeField]
    private LayerMask groundLayers;

    private bool _walking = false;
    private bool _drawBow = false;

    private Coroutine _weightTweenRoutine;

    public bool IsDrawArrowFullyPlayed {
        get {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);
            if (stateInfo.IsName("DrawBow"))
                return stateInfo.normalizedTime >= 1;
            return false;
        }
    }
    public float RollAnimationProgress {
        get {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Roll"))
                return stateInfo.normalizedTime;
            return 0;
        }
    }

    public event System.Action<float> OnAimAnimatinoChanged;
    public event System.Action<string> OnAnimationEventCalled;


    void Awake()
    {
        movement.OnJumpEvent += OnJump;
        movement.OnJumpEndEvent += OnJumpEnd;
        movement.OnRejumpEvent += OnRejump;
        movement.OnLandEvent += OnLand;
        movement.OnRollEvent += OnRoll;

        behaviour.OnDrawBow += OnDrawBow;
        behaviour.OnDrawBowEnd += OnDrawBowEnd;
        behaviour.OnDeath += OnDeath;
        behaviour.OnRevive += OnRevive;
        behaviour.OnBowUpgrade += OnBowUpgrade;

        float speedMultiplier = rollClip.length / movement.rollTime;
        animator.SetFloat(RollSpeed, speedMultiplier);

        behaviour.OnBowParameterChanged += ChangeAnimationAccordingToBowParameter;
    }

    void LateUpdate()
    {
        if (behaviour.IsDead)
            return;

        if (_drawBow)
        {
            OnAimAnimatinoChanged?.Invoke(animator.GetCurrentAnimatorStateInfo(1).normalizedTime);
            RotateChest();
        }


        if (movement.IsWalking != _walking)
        {
            _walking = movement.IsWalking;
            animator.SetBool(AnimKeyWalking, _walking);
            animator.ResetTrigger(AnimKeyEndJump);

            if (movement.IsWalking)
                stepDustParticle.Play();
            else
                stepDustParticle.Stop();
        }
    }


#region Movement Event
    void OnJump()
    {
        stepDustParticle.Stop();
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

    void OnLand()
    {
        // TODO: player land animation
        if (movement.IsWalking) stepDustParticle.Play();
    }

    void OnRoll()
    {
        animator.SetTrigger(AnimKeyRoll);
    }
#endregion


    void RotateChest()
    {
        Quaternion chestRotation = Quaternion.LookRotation(behaviour.CurrentRayHitPosition - chest.position, transform.up);
        chest.rotation = chestRotation * Quaternion.Lerp(chestRotationOffsetA, chestRotationOffsetB, movement.AngleLerpValue);

        bow.rotation = Quaternion.LookRotation(bow.position - rightHand.position, transform.up) * bowRotationOffset;

        Transform arrow = currentArrowPointer.Target;
        if (arrow)
            arrow.rotation = Quaternion.LookRotation(arrow.position - rightHand.position, arrow.up);
    }

    public void AnimationEvent(string eventName)
    {
        OnAnimationEventCalled?.Invoke(eventName);
    }


#region Player behaviour event
    void OnDrawBow()
    {
        _drawBow = true;
        animator.SetBool(AnimKeyDrawingBow, true);
        animator.SetFloat(AnimKeyWalkSpeed, drawBowSlowDown.Value);
    }

    void OnDrawBowEnd()
    {
        _drawBow = false;
        animator.SetBool(AnimKeyDrawingBow, false);
        animator.SetFloat(AnimKeyWalkSpeed, 1);
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
        animator.Play(AnimNameIdle, 0);
        animator.Play(AnimNameEmpty, 1);
    }

    void ChangeAnimationAccordingToBowParameter(BowParameter bowParameter, BowParameter newBowParameter)
    {
        float speedMultiplier = drawBowClip.length / bowParameter.FirstDrawDuration;
        animator.SetFloat(DrawBowSpeed, speedMultiplier);
    }

    void OnBowUpgrade()
    {
        levelUpEffect.AddWaitingList(new EffectReference.EffectQueue
        {
            Parent = transform,
            Position = transform.position,
            Rotation = transform.rotation,
            UseScaleTime = true,
        });
    }
#endregion
}
