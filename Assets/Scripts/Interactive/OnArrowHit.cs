using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MPack;

public class OnArrowHit : MonoBehaviour
{
    [SerializeField]
    private EffectReference effect;
    [SerializeField]
    private AudioClipSet sound;
    [SerializeField]
    private AudioSource audioSource;

    [field: SerializeField] public bool SetParent { get; protected set; }
    public bool HasEffect => effect;

    public UnityEvent OnTrigger;

    public void Trigger(Vector3 arrowPosition)
    {
        if (effect)
            effect.AddWaitingList(arrowPosition, Quaternion.identity);
        if (sound)
        {
            if (audioSource)
                audioSource.Play(sound);
            else
                AudioClipSetExtension.PlayClipAtPoint(sound, transform.position);
        }

        OnTrigger.Invoke();
    }
}
