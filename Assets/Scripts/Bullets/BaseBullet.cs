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
        BulletBillboards.ins.PutBullet(this);
    }

    public void Shoot(Vector3 velocity)
    {
        rigidbody.velocity = velocity;
    }
}
