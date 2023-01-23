using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;
using TMPro;
using DigitalRuby.Tween;

public class PlayerStatusHUD : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private AimCursor aimCursor;

    [Header("Fight Stage")]
    [SerializeField]
    private GameObject stageGameobject;
    [SerializeField]
    private LanguageText stageText;
    [SerializeField]
    private FillBarControl stageFillbar;
    private int _currentWaveIndex;
    private int _maxWaveIndex;

    [Space(8)]
    [SerializeField]
    private EventReference fightStartedEvent;
    [SerializeField]
    private EventReference fightWaveUpdateEvent;
    [SerializeField]
    private EventReference fightEndedEvent;

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
    private BowUpgradeUI[] bowUpgradeUIs;


    void Awake()
    {
        healthFill.SetFillAmount(1);
        healthEvent.InvokeFloatEvents += ChangeHealthAmount;

        stageText.languageProcess += OnFightStageLanguageTextProcess;

        fightStartedEvent.InvokeIntEvents += OnFightStarted;
        fightWaveUpdateEvent.InvokeIntEvents += OnFightWaveUpdated;
        fightEndedEvent.InvokeEvents += OnFightEnded;

        // TODO: bad
        if ((GameObject.FindWithTag(PlayerBehaviour.Tag) is var playerGameObject) && playerGameObject &&
            (playerGameObject.GetComponent<PlayerBehaviour>() is var player) && player)
            player.OnBowParameterChanged += UnlockBowUpgrade;
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
        aimCursor.SetAimDuration(bowParameter.FirstDrawDuration, bowParameter.SecondDrawDuration);

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


    string OnFightStageLanguageTextProcess(string content, TextMeshPro text, TextMeshProUGUI textGUI)
    {
        return content + string.Format(" {0}/{1}", _currentWaveIndex, _maxWaveIndex);
    }
    void OnFightStarted(int maxWaveIndex)
    {
        _maxWaveIndex = maxWaveIndex;
        _currentWaveIndex = 1;
        stageFillbar.SetFillAmount(1f / (float)maxWaveIndex);

        stageText.Reload();
        stageGameobject.SetActive(true);
    }
    void OnFightWaveUpdated(int currentWaveIndex)
    {
        _currentWaveIndex = currentWaveIndex;
        stageFillbar.SetFillAmount((float)_currentWaveIndex / (float)_maxWaveIndex);

        stageText.Reload();
    }
    void OnFightEnded()
    {
        stageGameobject.SetActive(false);
    }


    public void FadeOut(float duration)
    {
        gameObject.Tween("HUDFade", 1, 0, duration, TweenScaleFunctions.CubicEaseIn, (tweenData) => canvasGroup.alpha = tweenData.CurrentValue);
    }

    public void FadeIn(float duration)
    {
        gameObject.Tween("HUDFade", 0, 1, duration, TweenScaleFunctions.CubicEaseIn, (tweenData) => canvasGroup.alpha = tweenData.CurrentValue);
    }


    void OnDestroy()
    {
        healthEvent.InvokeFloatEvents -= ChangeHealthAmount;

        fightStartedEvent.InvokeIntEvents -= OnFightStarted;
        fightWaveUpdateEvent.InvokeIntEvents -= OnFightWaveUpdated;
        fightEndedEvent.InvokeEvents -= OnFightEnded;
    }

    [System.Serializable]
    public struct BowUpgradeUI
    {
        public BowParameter Upgrade;
        public Image Icon;
    }
}
