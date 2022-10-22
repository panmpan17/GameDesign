using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCore : MonoBehaviour
{
    public event System.Action OnDamageEvent;

    public void OnDamage()
    {
        Debug.Log("on damage");
        gameObject.SetActive(false);
        OnDamageEvent?.Invoke();
    }
}
