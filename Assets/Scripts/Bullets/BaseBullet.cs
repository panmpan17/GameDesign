using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class BaseBullet : BulletBehaviour
{
    [SerializeField]
    private SimpleTimer disapearTimer; 
    [SerializeField]
    private float damage;
    [SerializeField]
    private EffectReference shatterEffect;

    public event System.Action<Vector3> OnShoot;


    void FixedUpdate()
    {
        if (disapearTimer.FixedUpdateEnd)
        {
            disapearTimer.Reset();
            PutBackToPool();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(PlayerBehaviour.Tag))
        {
            PutBackToPool();
            shatterEffect?.AddWaitingList(transform.position, Quaternion.LookRotation(-rigidbody.velocity, Vector3.up));
            return;
        }

        var playerBehaviour = collider.GetComponent<PlayerBehaviour>();

        if (!playerBehaviour.TryToDamage())
            return;

        PutBackToPool();
        playerBehaviour.OnDamage(damage);
    }

    public override void Shoot(Vector3 velocity)
    {
        rigidbody.velocity = velocity;
        OnShoot?.Invoke(velocity);
    }


    public override void Reinstantiate()
    {
        base.Reinstantiate();
        disapearTimer.Reset();
    }
}
