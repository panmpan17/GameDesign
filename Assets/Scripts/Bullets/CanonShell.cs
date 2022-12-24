using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class CanonShell : BulletBehaviour
{
    [SerializeField]
    private float damageAmount;
    [SerializeField]
    private ParticleSystem expolosionParticle;
    [SerializeField]
    private LayerMask playerLayers;
    [SerializeField]
    private float explosionRadius;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClipSet explosionSound;

    private Vector3 _extraGravity;

    public event System.Action OnCollide;

    public override void Shoot(PhysicSimulate physicSimulate)
    {
        transform.position = physicSimulate.Position;
        rigidbody.mass = physicSimulate.Mass;
        rigidbody.velocity = physicSimulate.Velocity;

        _extraGravity = physicSimulate.Gravity - Physics.gravity;
    }

    void FixedUpdate()
    {
        rigidbody.velocity += (_extraGravity * Time.fixedDeltaTime) / rigidbody.mass;
        // Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        // rigidbody.AddForce(_extraGravity, ForceMode.Acceleration);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (expolosionParticle.isPlaying)
            return;

        OnCollide?.Invoke();
        OnCollide = null;
        audioSource.Play(explosionSound);

        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;

        expolosionParticle.Play();
        StartCoroutine(WaitParticleStop());
    }

    IEnumerator WaitParticleStop()
    {
        bool isHit = false;
        while (!expolosionParticle.isStopped)
        {
            if (isHit)
            {
                yield return null;
                continue;
            }
            

            Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, playerLayers);
            if (hits.Length <= 0)
            {
                yield return null;
                continue;
            }

            var playerBehaviour = hits[0].GetComponent<PlayerBehaviour>();

            if (!playerBehaviour.TryToDamage())
            {
                yield return null;
                continue;
            }

            playerBehaviour.OnDamage(damageAmount);
            isHit = true;

            yield return null;
        }

        PutBackToPool();
    }


    public override void DeactivateObj(Transform collectionTransform)
    {
        base.DeactivateObj(collectionTransform);
        OnCollide?.Invoke();
        OnCollide = null;
    }

    public override void Reinstantiate()
    {
        base.Reinstantiate();
        rigidbody.isKinematic = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
