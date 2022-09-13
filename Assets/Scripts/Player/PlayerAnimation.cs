using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private const string AnimKeyWalking = "Walking";
    private const string AnimKeyJump = "Jump";
    private const string AnimKeyEndJump = "EndJump";
    private const string AnimKeyDrawBow = "DrawBow";
    private const string AnimKeyWalkSpeed = "WalkSpeed";
    
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private PlayerBehaviour behaviour;

    private int _walkingKey;
    private int _jumpKey;
    private int _jumpEndKey;

    private bool _walking;


    void Awake()
    {
        movement.OnJump += OnJump;
        movement.OnJumpEnd += OnJumpEnd;
        FindAnimatorKey();
    }

    void FindAnimatorKey()
    {
        _walkingKey = Animator.StringToHash(AnimKeyWalking);
        _jumpKey = Animator.StringToHash(AnimKeyJump);
        _jumpEndKey = Animator.StringToHash(AnimKeyEndJump);
    }

    void LateUpdate()
    {
        if (movement.IsWalking != _walking)
        {
            _walking = movement.IsWalking;
            animator.SetBool(_walkingKey, _walking);
        }
    }

    void OnJump()
    {
        Debug.Log("jump");
        animator.ResetTrigger(_jumpEndKey);
        animator.SetTrigger(_jumpKey);
    }

    void OnJumpEnd()
    {
        Debug.Log("end jump");
        animator.ResetTrigger(_jumpKey);
        animator.SetTrigger(_jumpEndKey);
    }
}
