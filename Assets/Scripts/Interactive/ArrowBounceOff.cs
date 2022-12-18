using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBounceOff : MonoBehaviour
{
    public const string Tag = "BounceOff";

    public AudioSource audioSource;
    public AudioClipSet bounceSound;

    public event System.Action OnBounceOffEvent;

    public void OnBounceOff() => OnBounceOffEvent?.Invoke();
}
