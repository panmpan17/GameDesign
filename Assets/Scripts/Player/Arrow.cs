using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

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
    private float speed;
    [SerializeField]
    private Timer ignoreGravityTimer;
    [SerializeField]
    private float extraDrawDuration;
    private float _baseTime;

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

        _baseTime = ignoreGravityTimer.TargetTime;
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

    public void Shoot(Vector3 targetPosition, float extraProgress)
    {
        enabled = true;

        ignoreGravityTimer.TargetTime = _baseTime + (extraDrawDuration * extraProgress);

        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, transform.up);

        rigidbody.velocity = transform.forward * speed;
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
        enabled = false;

        if (otherTransform.CompareTag(SlimeBehaviourTreeRunner.Tag))
        {
            transform.SetParent(otherTransform);
        }
        else if (otherTransform.CompareTag(SlimeCore.Tag))
        {
            transform.SetParent(otherTransform.parent);
            var slimeCore = otherTransform.GetComponent<SlimeCore>();
            slimeCore.OnDamage();

            hit.AddWaitingList(transform.position, Quaternion.LookRotation(-rigidbody.velocity, Vector3.up));
            audioSource.Play(hitSound);
        }
        else if (otherTransform.CompareTag(InteractiveBase.Tag))
        {
            var interactive = otherTransform.GetComponent<InteractiveBase>();
            interactive.ArrowHit();
            gameObject.SetActive(false);
        }

        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;

        trail.emitting = false;
        particle.Stop();
        rigidbody.useGravity = false;
    }
}
