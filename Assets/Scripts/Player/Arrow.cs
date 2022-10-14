using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class Arrow : MonoBehaviour, IPoolableObj
{
    private const float HeightLimit = 300;
    private const float LowLimit = -30;

    [SerializeField]
    private float speed;
    [SerializeField]
    private Timer ignoreGravityTimer;
    [SerializeField]
    private new Rigidbody rigidbody;
    [SerializeField]
    private TrailRenderer trail;

    public void Instantiate()
    {
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector3.zero;
        trail.emitting = false;
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

    public void Shoot(Vector3 targetPosition)
    {
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, transform.up);

        rigidbody.velocity = transform.forward * speed;
        rigidbody.isKinematic = false;
        trail.emitting = true;

        ignoreGravityTimer.Reset();
    }

    void FixedUpdate()
    {
        if (ignoreGravityTimer.Running && ignoreGravityTimer.FixedUpdateEnd)
        {
            ignoreGravityTimer.Running = false;
            rigidbody.useGravity = true;
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
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;

        if (otherTransform.CompareTag("Slime"))
        {
            transform.SetParent(otherTransform);
        }
        else if (otherTransform.CompareTag("SlimeCore"))
        {
            transform.SetParent(otherTransform.parent);
            var slimeCore = otherTransform.GetComponent<SlimeCore>();
            slimeCore.OnDamage();
        }

        trail.emitting = false;
        rigidbody.useGravity = false;
    }
}
