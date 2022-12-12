using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class BossHealthBar : MonoBehaviour
{
    [SerializeField]
    private EventReference showEvent;
    [SerializeField]
    private EventReference healthChangeEvent;
    [SerializeField]
    private GameObject healthBar;
    [SerializeField]
    private FillBarControl barControl;

    [SerializeField]
    private ShakeTween shakeTween;

    void Awake()
    {
        showEvent.InvokeBoolEvents += healthBar.SetActive;
        healthChangeEvent.InvokeFloatEvents += BossHealthChanged;
    }

    void OnDestroy()
    {
        showEvent.InvokeBoolEvents -= healthBar.SetActive;
        healthChangeEvent.InvokeFloatEvents -= BossHealthChanged;
    }

    void BossHealthChanged(float amount)
    {
        barControl.SetFillAmount(amount);
        shakeTween.Trigger();
    }
}
