using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PickupFlower : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClipSet pickupSound;

    public UnityEvent OnPickup;

    public void Pickup()
    {
        audioSource.PlayOneShot(pickupSound);
        gameObject.SetActive(false);
        OnPickup.Invoke();
    }

    public void OnSaveRestore()
    {
        gameObject.SetActive(false);
    }
}
