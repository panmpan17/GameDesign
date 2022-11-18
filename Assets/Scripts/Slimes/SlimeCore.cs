using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class SlimeCore : MonoBehaviour
{
    public const string Tag = "SlimeCore";


    [SerializeField]
    private EffectReference hitEffect;

    public event System.Action OnDamageEvent;

    public void OnDamage()
    {
        gameObject.SetActive(false);
        OnDamageEvent?.Invoke();

        if (hitEffect)
            hitEffect.AddWaitingList(transform.position, Quaternion.identity);
    }
}
