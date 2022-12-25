using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using XnodeBehaviourTree;


public class Arrow : MonoBehaviour, IPoolableObj
{
    private const float HeightLimit = 300;
    private const float LowLimit = -30;

    [Header("Reference")]
    [SerializeField]
    private new Rigidbody rigidbody;
    [SerializeField]
    private new Collider collider;
    [SerializeField]
    private TrailRenderer trail;
    [SerializeField]
    private ParticleSystem particle;
    private ParticleSystem.MainModule particleMain;
    private ParticleSystem.MinMaxGradient startColor;

    [SerializeField]
    private EffectReference hitGroundEffect;

    [Header("Parameter")]
    [SerializeField]
    private AnimationCurveReference gravityScaleCurve;
    private Timer ignoreGravityTimer;

    [SerializeField]
    private ColorReference particleColor;
    [SerializeField]
    private ColorReference particleExtraColor;

    [Header("Sound")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClipSet bounceOffSound;

    public bool TrailEmmiting {
        get => trail.emitting;
        set => trail.emitting = value;
    }


    public void Instantiate()
    {
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector3.zero;
        trail.emitting = false;
        particle.Stop();
        particleMain = particle.main;

        // _baseTime = ignoreGravityTimer.TargetTime;
    }

    public void DeactivateObj(Transform collectionTransform)
    {
        enabled = false;
        gameObject.SetActive(false);
        transform.SetParent(collectionTransform);

        trail.emitting = false;
        particle.Stop();
    }

    public void Reinstantiate()
    {
        gameObject.SetActive(true);
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector3.zero;
        audioSource.Stop();
    }

    public void Shoot(Vector3 velocity, float ignoreGravityDuration, float extraProgress)
    {
        enabled = true;
        collider.enabled = true;

        ignoreGravityTimer.TargetTime = ignoreGravityDuration;

        transform.rotation = Quaternion.LookRotation(velocity, transform.up);

        rigidbody.velocity = velocity;//transform.forward * speed;
        rigidbody.isKinematic = false;
        trail.emitting = true;

        Color effectColor = Color.Lerp(particleColor.Value, particleExtraColor.Value, extraProgress);;

        startColor.color = Color.Lerp(particleColor.Value, particleExtraColor.Value, extraProgress);
        particleMain.startColor = startColor;
        particle.Play();

        trail.startColor = effectColor;
        trail.endColor = effectColor;

        ignoreGravityTimer.Reset();
    }

    void FixedUpdate()
    {
        if (ignoreGravityTimer.Running)
        {
            Vector3 desireGravity = Vector3.Lerp(Vector3.zero, Physics.gravity, gravityScaleCurve.Value.Evaluate(ignoreGravityTimer.Progress));
            rigidbody.velocity += desireGravity * Time.fixedDeltaTime;

            if (ignoreGravityTimer.FixedUpdateEnd)
            {
                ignoreGravityTimer.Running = false;
                rigidbody.useGravity = true;
            }
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(rigidbody.velocity, transform.up);
        }
        if (transform.position.y > HeightLimit || transform.position.y < LowLimit)
            gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!rigidbody.isKinematic)
            HandleHit(collider.transform);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!rigidbody.isKinematic)
            HandleHit(collision.transform);
    }

    void HandleHit(Transform otherTransform)
    {
        bool playHitGroundEffect = true;
        if (otherTransform.GetComponent<OnArrowHit>() is var arrowHit && arrowHit)
        {
            playHitGroundEffect = !arrowHit.HasEffect;
            arrowHit.Trigger(transform.position);

            if (arrowHit.SetParent)
            {
                transform.SetParent(otherTransform);
                StopArrow();
                return;
            }
        }

        if (otherTransform.CompareTag(ArrowBounceOff.Tag))
        {
            bool playBounceOffSound = false;

            if (otherTransform.GetComponent<ArrowBounceOff>() is var bounceOff && bounceOff)
            {
                bounceOff.OnBounceOff();
                playBounceOffSound = bounceOff.TryPlayBounceSound();
            }
            BounceOffArrow(playBounceOffSound);
            return;
        }

        if (otherTransform.CompareTag(SlimeBehaviourTreeRunner.Tag))
        {
            OnHitSlimeBody(otherTransform);
            return;
        }

        if (otherTransform.CompareTag(SlimeCore.Tag))
        {
            OnHitSlimeCore(otherTransform);
            return;
        }

        StopArrow();

        if (playHitGroundEffect)
        {
            hitGroundEffect.AddWaitingList(transform.position, transform.rotation);
        }
    }

    void BounceOffArrow(bool playBounceOffSound)
    {
        enabled = false;
        rigidbody.useGravity = true;

        if (playBounceOffSound)
            audioSource.Play(bounceOffSound);
    }

    void OnHitSlimeBody(Transform otherTransform)
    {
        var slime = otherTransform.GetComponent<SlimeBehaviour>();
        if (slime.ArrowBounceOff)
        {
            slime.OnBounceOff();
            BounceOffArrow(false);
            return;
        }

        transform.SetParent(otherTransform);
        StopArrow();

        slime.OnArrowHitBody(this);
    }

    void OnHitSlimeCore(Transform otherTransform)
    {
        transform.SetParent(otherTransform.parent);

        if (!enabled)
            return;

        var slimeCore = otherTransform.GetComponent<SlimeCore>();
        slimeCore.OnDamage();

        StopArrow();
    }


    void StopArrow()
    {
        enabled = false;
        collider.enabled = false;
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;

        trail.emitting = false;
        particle.Stop();
    }
}
