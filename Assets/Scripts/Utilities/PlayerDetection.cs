using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public System.Action OnPlayerEnter;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(PlayerBehaviour.Tag))
        {
            OnPlayerEnter?.Invoke();
        }
    }
}
