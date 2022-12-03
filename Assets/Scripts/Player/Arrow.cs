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
    private TrailRenderer trail;
    [SerializeField]
    private ParticleSystem particle;
    private ParticleSystem.MainModule particleMain;
    private ParticleSystem.MinMaxGradient startColor;

    [SerializeField]
    private EffectReference hit;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClipSet hitSound;

    [Header("Parameter")]
    [SerializeField]
    private AnimationCurveReference gravityScaleCurve;
    private Timer ignoreGravityTimer;

    [SerializeField]
    private ColorReference particleColor;
    [SerializeField]
    private ColorReference particleExtraColor;

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
    }

    public void Shoot(Vector3 velocity, float ignoreGravityDuration, float extraProgress)
    {
        enabled = true;

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
        var OnArrowHit = otherTransform.GetComponent<OnArrowHit>();
        if (OnArrowHit)
            OnArrowHit.Trigger(transform.position);

        if (otherTransform.CompareTag(SlimeBehaviourTreeRunner.Tag))
        {
            enabled = false;
            rigidbody.useGravity = true;
            return;
        }
        else if (otherTransform.CompareTag(SlimeCore.Tag))
        {
            transform.SetParent(otherTransform.parent);

            if (enabled)
            {
                var slimeCore = otherTransform.GetComponent<SlimeCore>();
                slimeCore.OnDamage();

                audioSource.Play(hitSound);
            }
        }
        else if (otherTransform.CompareTag(InteractiveBase.Tag))
        {
            var interactive = otherTransform.GetComponent<InteractiveBase>();
            interactive.ArrowHit();
            gameObject.SetActive(false);
        }

        enabled = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;

        trail.emitting = false;
        particle.Stop();
        rigidbody.useGravity = false;
    }
}
