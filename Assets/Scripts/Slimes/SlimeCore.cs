using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCore : MonoBehaviour
{
    public const string Tag = "SlimeCore";

    public event System.Action OnDamageEvent;

    public void OnDamage()
    {
        gameObject.SetActive(false);
        OnDamageEvent?.Invoke();
    }
}
