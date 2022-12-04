using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;
using TMPro;

public class PlayerStatusHUD : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField]
    private Image aimProgressFrame;
    [SerializeField]
    private FillBarControl aimProgressBar;
    [SerializeField]
    private EventReference aimProgressEvent;

    [SerializeField]
    private Color aimFrameUnfocusColor;
    [SerializeField]
    private Color aimFrameFocusColor;

    [Header("Health")]
    [SerializeField]
    private EventReference healthEvent;
    [SerializeField]
    private FillBarControl healthFill;
    [SerializeField]
    private ShakeTween damageShake;
    [SerializeField]
    private ColorTween healShine;
    [SerializeField]
    private Timer healAnimateTimer;

    private Coroutine _healAnimation;

    [Header("Upgrades")]
    [SerializeField]
    private GameObject firstUpgrade;

    [Header("Inventory")]
    [SerializeField]
    private EventReference coreChangeEvent;
    [SerializeField]
    private TextMeshProUGUI coreCountText;

    [SerializeField]
    private EventReference appleChangeEvent;
    [SerializeField]
    private TextMeshProUGUI appleCountText;

    void Awake()
    {
        aimProgressBar.SetFillAmount(0);
        healthFill.SetFillAmount(1);

        coreChangeEvent.InvokeIntEvents += ChangeCoreCount;
        appleChangeEvent.InvokeIntEvents += ChangeAppleCount;
        aimProgressEvent.InvokeFloatEvents += ChangeAimProgress;
        healthEvent.InvokeFloatEvents += ChangeHealthAmount;
    }

    void OnDestroy()
    {
        coreChangeEvent.InvokeIntEvents -= ChangeCoreCount;
        appleChangeEvent.InvokeIntEvents -= ChangeAppleCount;
        aimProgressEvent.InvokeFloatEvents -= ChangeAimProgress;
    }

    void ChangeCoreCount(int count) => coreCountText.text = count.ToString();
    void ChangeAppleCount(int count) => appleCountText.text = count.ToString();

    void ChangeAimProgress(float progress)
    {
        aimProgressFrame.color = progress > 0 ? aimFrameFocusColor : aimFrameUnfocusColor;
        aimProgressBar.SetFillAmount(progress / 2);
    }

    void ChangeHealthAmount(float newAmount)
    {
        if (_healAnimation != null)
            StopCoroutine(_healAnimation);


        if (newAmount > healthFill.Amount)
        {
            _healAnimation = StartCoroutine(AnimateHealthHeal(newAmount));
        }
        else if (newAmount < healthFill.Amount)
        {
            healthFill.SetFillAmount(newAmount);
            damageShake.Trigger();
        }
    }

    IEnumerator AnimateHealthHeal(float newAmount)
    {
        float oldAmount = healthFill.Amount;

        healShine.enabled = true;

        healAnimateTimer.Reset();
        while (!healAnimateTimer.UpdateEnd)
        {
            healthFill.SetFillAmount(Mathf.Lerp(oldAmount, newAmount, healAnimateTimer.Progress));
            yield return null;
        }
        healthFill.SetFillAmount(newAmount);

        healShine.enabled = false;
        healShine.ResetTween(0);
    }
}
