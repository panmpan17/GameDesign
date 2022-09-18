using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class BaseBullet : MonoBehaviour, IPoolableObj
{
    [SerializeField]
    private new Rigidbody rigidbody;
    [SerializeField]
    private SimpleTimer disapearTimer; 
    [SerializeField]
    private float damage;

    public void DeactivateObj(Transform collectionTransform) {
        transform.SetParent(collectionTransform);
        gameObject.SetActive(false);
    }
    public void Instantiate() {}
    public void Reinstantiate() {
        gameObject.SetActive(true);
        disapearTimer.Reset();
    }

    void FixedUpdate()
    {
        if (disapearTimer.FixedUpdateEnd)
        {
            disapearTimer.Reset();
            BulletBillboards.ins.PutBullet(this);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(PlayerBehaviour.Tag))
        {
            BulletBillboards.ins.PutBullet(this);
            return;
        }

        var playerBehaviour = collider.GetComponent<PlayerBehaviour>();

        if (playerBehaviour.Movement.IsRolling)
            return;

        BulletBillboards.ins.PutBullet(this);
        playerBehaviour.OnDamage(damage);
    }

    public void Shoot(Vector3 velocity)
    {
        rigidbody.velocity = velocity;
    }
}
