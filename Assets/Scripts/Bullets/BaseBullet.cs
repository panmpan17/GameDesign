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
            return;
        }

        var playerBehaviour = collider.GetComponent<PlayerBehaviour>();

        if (!playerBehaviour.CanDamage)
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
