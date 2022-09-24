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

        if (playerBehaviour.Movement.IsRolling)
            return;

        PutBackToPool();
        playerBehaviour.OnDamage(damage);
    }

    public override void Shoot(Vector3 velocity)
    {
        rigidbody.velocity = velocity;
    }


    public override void Reinstantiate()
    {
        base.Reinstantiate();
        disapearTimer.Reset();
    }
}
