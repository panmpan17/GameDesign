using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class PlayerSound : MonoBehaviour
{
    [SerializeField]
    private new PlayerAnimation animation;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private PlayerBehaviour behaviour;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioSource oneShotAudioSource;
    
    [Header("Bow")]
    [SerializeField]
    private AudioClipSet bowDrawClip;
    [SerializeField]
    private AudioClipSet bowShootClip;
    [SerializeField]
    private RangeReference bowShootVolumeRange;

    [Header("Clips")]
    [SerializeField]
    private AudioClipSet runClip;
    [SerializeField]
    private AudioClipSet walkClip;
    [SerializeField]
    private AudioClipSet jumpClip;
    [SerializeField]
    private AudioClipSet landClip;
    [SerializeField]
    private AudioClipSet pickItemClip;
    [SerializeField]
    private AudioClipSet levelUpClip;
    [SerializeField]
    private AudioClipSet hurtClip;
    [SerializeField]
    private AudioClipSet rollClip;


    void Awake()
    {
        movement.OnJumpEvent += OnJump;
        movement.OnRejumpEvent += OnJump;
        movement.OnJumpEndEvent += OnLand;
        movement.OnRollEvent += OnRoll;

        behaviour.OnDrawBow += OnDrawBow;
        behaviour.OnDrawBowEnd += OnDrawBowEnd;
        behaviour.OnBowShoot += OnBowShoot;
        behaviour.OnPickItem += OnPickItem;
        behaviour.OnBowUpgrade += OnBowUpgrade;
        behaviour.OnHurt += OnHurt;

        animation.OnAnimationEventCalled += OnAnimationEvent;
    }


    void OnJump() => oneShotAudioSource.PlayOneShot(jumpClip);
    void OnLand() => oneShotAudioSource.PlayOneShot(landClip);
    void OnRoll() => oneShotAudioSource.PlayOneShot(rollClip);

    void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "RunStep":
                oneShotAudioSource.PlayOneShot(runClip);
                break;
            case "WalkStep":
                oneShotAudioSource.PlayOneShot(walkClip);
                break;
        }
    }

    void OnDrawBow() => audioSource.Play(bowDrawClip);
    void OnDrawBowEnd() => audioSource.Stop();

    void OnBowShoot(float extraDrawProgress) => oneShotAudioSource.PlayOneShot(bowShootClip, volume: bowShootVolumeRange.Lerp(extraDrawProgress));
    
    void OnPickItem() => oneShotAudioSource.PlayOneShot(pickItemClip);

    void OnBowUpgrade() => oneShotAudioSource.PlayOneShot(levelUpClip);

    void OnHurt() => oneShotAudioSource.PlayOneShot(hurtClip);
}
