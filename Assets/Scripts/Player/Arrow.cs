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
    private EffectReference hit;

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
    [SerializeField]
    private AudioClipSet bodyHit;

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
        if (otherTransform.GetComponent<OnArrowHit>() is var arrowHit && arrowHit)
        {
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
            var bounceOff = otherTransform.GetComponent<ArrowBounceOff>();
            BounceOffArrow(bounceOff);
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
    }

    void BounceOffArrow(ArrowBounceOff bounceOff=null)
    {
        enabled = false;
        rigidbody.useGravity = true;

        AudioSource _audioSource = audioSource;
        AudioClipSet sound = bounceOffSound;

        if (bounceOff)
        {
            if (bounceOff.audioSource) _audioSource = bounceOff.audioSource;
            if (bounceOff.bounceSound) sound = bounceOff.bounceSound;
        }

        _audioSource.Play(sound);
    }

    void OnHitSlimeBody(Transform otherTransform)
    {
        var slime = otherTransform.GetComponent<SlimeBehaviour>();
        if (slime.ArrowBounceOff)
        {
            BounceOffArrow();
            return;
        }

        transform.SetParent(otherTransform);
        StopArrow();

        slime.ShakeArrow(this);
        audioSource.Play(bodyHit);
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
