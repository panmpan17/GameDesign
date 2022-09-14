using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class Arrow : MonoBehaviour, IPoolableObj
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private new Rigidbody rigidbody;

    public void Instantiate()
    {
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector3.zero;
    }

    public void DeactivateObj(Transform collectionTransform)
    {
        gameObject.SetActive(false);
        transform.SetParent(collectionTransform);
    }

    public void Reinstantiate()
    {
        gameObject.SetActive(true);
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector3.zero;
    }

    public void Shoot()
    {
        rigidbody.velocity = transform.forward * speed;
        rigidbody.isKinematic = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;
    }
}
