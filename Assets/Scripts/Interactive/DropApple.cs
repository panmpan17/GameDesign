using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(InteractiveBase)), RequireComponent(typeof(Rigidbody))]
public class DropApple : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onHitGround;

    private InteractiveBase _interactiveBase;
    private Rigidbody _rigidbody;

    void Awake()
    {
        _interactiveBase = GetComponent<InteractiveBase>();
        _interactiveBase.OnArrowHit.AddListener(OnArrowHit);

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
    }

    void OnArrowHit()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit ground");
        onHitGround.Invoke();
    }
}
