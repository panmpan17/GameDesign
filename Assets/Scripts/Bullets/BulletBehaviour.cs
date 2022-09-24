using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public abstract class BulletBehaviour : MonoBehaviour, IPoolableObj
{
    [SerializeField]
    protected new Rigidbody rigidbody;

    public delegate void PutFunction<in BulletBehaviour>(BulletBehaviour bullet);
    public PutFunction<BulletBehaviour> putFunction;

    public virtual void DeactivateObj(Transform collectionTransform)
    {
        gameObject.SetActive(false);
        transform.SetParent(collectionTransform);
    }
    public virtual void Instantiate() { }
    public virtual void Reinstantiate()
    {
        gameObject.SetActive(true);
    }

    public virtual void Shoot(Vector3 velocity) => new System.NotImplementedException();
    public virtual void Shoot(PhysicSimulate physicSimulate) => new System.NotImplementedException();

    protected void PutBackToPool()
    {
        putFunction(this);
    }
}
