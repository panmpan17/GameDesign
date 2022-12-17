using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnPlayerEnter;
    [SerializeField]
    private UnityEvent OnPlayerExit;

    public event System.Action OnPlayerEnterEvent;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(PlayerBehaviour.Tag))
        {
            OnPlayerEnterEvent?.Invoke();
            OnPlayerEnter.Invoke();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag(PlayerBehaviour.Tag))
        {
            OnPlayerExit.Invoke();
        }
    }
}
