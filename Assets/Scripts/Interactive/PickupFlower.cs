using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PickupFlower : MonoBehaviour
{
    public UnityEvent OnPickup;

    public void Pickup()
    {
        gameObject.SetActive(false);
        OnPickup.Invoke();
    }
}
