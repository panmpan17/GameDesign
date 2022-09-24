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

    public override void Shoot(PhysicSimulate physicSimulate)
    {
        transform.position = physicSimulate.Position;
        rigidbody.mass = physicSimulate.Mass;
        rigidbody.velocity = physicSimulate.Velocity;
    }

    void OnTriggerEnter(Collider collider)
    {
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

            if (playerBehaviour.Movement.IsRolling)
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
