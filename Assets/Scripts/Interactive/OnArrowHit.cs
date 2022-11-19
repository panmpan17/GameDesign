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

    public UnityEvent OnTrigger;

    public void Trigger(Vector3 arrowPosition)
    {
        if (effect)
            effect.AddWaitingList(arrowPosition, Quaternion.identity);
        if (sound)
            AudioClipSetExtension.PlayClipAtPoint(sound, transform.position);
        OnTrigger.Invoke();
    }
}
