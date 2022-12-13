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
    private CanvasGroup aimProgressFrame;
    [SerializeField]
    private Image fill;
    [SerializeField]
    private EventReference aimProgressEvent;

    [SerializeField]
    private float aimFrameUnfocusAlpha;
    [SerializeField]
    private float aimFrameFocusAlpha;

    [Header("Aim Ring Color")]
    [SerializeField]
    private Image firstAim;
    [SerializeField]
    private Image secondAim;
    [SerializeField]
    private Color color1;
    [SerializeField]
    private Color color2;
    [SerializeField]
    private float min;
    [SerializeField]
    private float max;
    [SerializeField]
    private AnimationCurve curve;
    private float _firstAimPercentage;

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
    // [SerializeField]
    // private GameObject firstUpgrade;
    [SerializeField]
    private BowUpgradeUI[] bowUpgradeUIs;

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
        healthFill.SetFillAmount(1);

        coreChangeEvent.InvokeIntEvents += ChangeCoreCount;
        appleChangeEvent.InvokeIntEvents += ChangeAppleCount;
        aimProgressEvent.InvokeFloatEvents += ChangeAimProgress;
        healthEvent.InvokeFloatEvents += ChangeHealthAmount;

        // TODO: bad
        var player = GameObject.FindWithTag(PlayerBehaviour.Tag).GetComponent<PlayerBehaviour>();
        if (player)
        {
            player.OnBowParameterChanged += UnlockBowUpgrade;
        }

        fill.fillAmount = 0;

        aimProgressFrame.alpha = aimFrameUnfocusAlpha;
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
        if (progress <= 0)
        {
            aimProgressFrame.alpha = aimFrameUnfocusAlpha;
            fill.fillAmount = 0;
            return;
        }

        aimProgressFrame.alpha = aimFrameFocusAlpha;

        if (progress <= 1)
        {
            fill.fillAmount = Mathf.Lerp(0, _firstAimPercentage, progress);
            fill.color = firstAim.color;
        }
        else
        {
            float progressMinus1 = progress - 1;
            fill.fillAmount = Mathf.Lerp(_firstAimPercentage, 1, progressMinus1);
            fill.color = Color.Lerp(color1, color2, progressMinus1);
        }
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


    public void UnlockBowUpgrade(BowParameter bowParameter, BowParameter newBowParameter)
    {
        SetAimDuration(bowParameter.FirstDrawDuration, bowParameter.SecondDrawDuration);

        for (int i = 0; i < bowUpgradeUIs.Length; i++)
        {
            BowUpgradeUI upgradeUI = bowUpgradeUIs[i];
            if (upgradeUI.Upgrade == newBowParameter)
            {
                upgradeUI.Icon.color = Color.white;
                return;
            }
        }
    }


    void SetAimDuration(float firstAimDuration, float secondAimDuration)
    {
        float total = firstAimDuration + secondAimDuration;
        _firstAimPercentage = firstAimDuration / total;
        float secondPercentage = secondAimDuration / total;
        firstAim.fillAmount = _firstAimPercentage;
        secondAim.fillAmount = secondPercentage;
        secondAim.transform.rotation = Quaternion.Euler(0, 0, _firstAimPercentage * -360);

        secondAim.material.SetFloat("_Degree", Mathf.Lerp(min, max, curve.Evaluate(secondPercentage)));
    }


    [System.Serializable]
    public struct BowUpgradeUI
    {
        public BowParameter Upgrade;
        public Image Icon;
    }
}
