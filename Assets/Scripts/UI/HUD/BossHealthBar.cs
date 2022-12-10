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


    void Awake()
    {
        showEvent.InvokeBoolEvents += healthBar.SetActive;
        healthChangeEvent.InvokeFloatEvents += barControl.SetFillAmount;
    }

    void OnDestroy()
    {
        showEvent.InvokeBoolEvents -= healthBar.SetActive;
        healthChangeEvent.InvokeFloatEvents -= barControl.SetFillAmount;
    }
}
