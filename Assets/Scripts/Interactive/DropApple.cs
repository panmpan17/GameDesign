using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody))]
public class DropApple : MonoBehaviour
{
    [SerializeField]
    private AudioClipSet dropGroundSound;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private UnityEvent onHitGround;

    private Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
    }

    public void OnArrowHit()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.isStatic)
        {
            onHitGround.Invoke();
            audioSource.Play(dropGroundSound);
        }
    }
}
