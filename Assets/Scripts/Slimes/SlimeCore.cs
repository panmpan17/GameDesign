using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class SlimeCore : MonoBehaviour
{
    public const string Tag = "SlimeCore";


    [SerializeField]
    private EffectReference hitEffect;

    public event System.Action<SlimeCore> OnDamageEvent;

    public void OnDamage()
    {
        if (!enabled)
            return;

        gameObject.SetActive(false);
        OnDamageEvent?.Invoke(this);

        if (hitEffect)
            hitEffect.AddWaitingList(transform.position, Quaternion.identity);
    }
}
