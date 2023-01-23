using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;
using TMPro;


public class InventoryGainUI : MonoBehaviour
{
    private static readonly int FadeInHashKey = Animator.StringToHash("FadeIn");
    private static readonly int FadeOutHashKey = Animator.StringToHash("FadeOut");

    public static InventoryGainUI ins;

    [SerializeField]
    private Inventory playerInventory;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float waitDuration;

    [Header("Dialog title")]
    [SerializeField]
    private LanguageText titleLanguageText;
    [SerializeField]
    [LauguageID]
    private int treasureOpenTitle;
    [SerializeField]
    [LauguageID]
    private int upgradeGetTitle;

    [Header("Apple & Core")]
    [SerializeField]
    private Vector2 singleApplePosition;
    [SerializeField]
    private Vector2 singleCorePosition;
    [SerializeField]
    private Vector2 bothApplePosition;
    [SerializeField]
    private Vector2 bothCorePosition;

    [Space(8)]
    [SerializeField]
    private RectTransform appleGain;
    [SerializeField]
    private TextMeshProUGUI appleCountText;
    [SerializeField]
    private RectTransform coreGain;
    [SerializeField]
    private TextMeshProUGUI coreCountText;

    [Header("Upgrade")]
    [SerializeField]
    private RectTransform upgradeRectTransform;
    [SerializeField]
    private Image upgradeIconImage;
    [SerializeField]
    private LanguageText upgradeTitleLanguage;

    private Canvas _canvas;


    void Awake()
    {
        ins = this;
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = animator.enabled = false;
        upgradeTitleLanguage.languageProcess += UpgradeTitleTextChanged;
    }

    public void ShowBowUpgrade(BowParameter bowUpgrade)
    {
        PauseTimeTrack.Pause(this);
        _canvas.enabled = animator.enabled = true;

        titleLanguageText.ChangeId(upgradeGetTitle);

        upgradeIconImage.sprite = bowUpgrade.Icon;
        upgradeTitleLanguage.ChangeId(bowUpgrade.AquireAnnounceNameID);
        upgradeRectTransform.gameObject.SetActive(true);

        appleGain.gameObject.SetActive(false);
        coreGain.gameObject.SetActive(false);

        StartCoroutine(C_StartFade(bowUpgrade));
    }

    public void ShowInventoryGain(int appleGainAmount, int coreGainAmount)
    {
        PauseTimeTrack.Pause(this);
        _canvas.enabled = animator.enabled = true;

        titleLanguageText.ChangeId(treasureOpenTitle);

        upgradeRectTransform.gameObject.SetActive(false);

        if (appleGainAmount > 0 && coreGainAmount > 0)
        {
            appleCountText.text = "x" + appleGainAmount.ToString();
            appleGain.anchoredPosition = bothApplePosition;
            appleGain.gameObject.SetActive(true);

            coreCountText.text = "x" + coreGainAmount.ToString();
            coreGain.anchoredPosition = bothCorePosition;
            coreGain.gameObject.SetActive(true);
        }
        else if (appleGainAmount > 0)
        {
            appleCountText.text = "x" + appleGainAmount.ToString();
            appleGain.anchoredPosition = singleApplePosition;
            appleGain.gameObject.SetActive(true);

            coreGain.gameObject.SetActive(false);
        }
        else //coreGainAmount > 0
        {
            coreCountText.text = "x" + coreGainAmount.ToString();
            coreGain.anchoredPosition = singleCorePosition;
            coreGain.gameObject.SetActive(true);

            appleGain.gameObject.SetActive(false);
        }

        StartCoroutine(C_StartFade(appleGainAmount, coreGainAmount));
    }


    IEnumerator C_StartFade(BowParameter bowParameter)
    {
        animator.Play(FadeInHashKey);
        yield return new WaitForSecondsRealtime(waitDuration);
        animator.Play(FadeOutHashKey);
        yield return new WaitForSecondsRealtime(0.8f);
        _canvas.enabled = animator.enabled = false;

        PauseTimeTrack.Unpause(this);
        GameManager.ins.Player.UpgradeBow(bowParameter);
    }

    IEnumerator C_StartFade(int appleAmount, int coreAmount)
    {
        animator.Play(FadeInHashKey);
        yield return new WaitForSecondsRealtime(waitDuration);
        animator.Play(FadeOutHashKey);
        yield return new WaitForSecondsRealtime(0.8f);
        _canvas.enabled = animator.enabled = false;

        PauseTimeTrack.Unpause(this);
        playerInventory.AppleCount += appleAmount;
        playerInventory.CoreCount += coreAmount;
    }

    string UpgradeTitleTextChanged(string text, TextMeshPro textMeshPro, TextMeshProUGUI textMeshProUGUI)
    {
        SetText(upgradeRectTransform, textMeshProUGUI, text);
        return text;
    }


    public void SetText(RectTransform textParentRectTransform, TextMeshProUGUI textComponent, string text)
    {
        RectTransform titleTextRectTransform = textComponent.rectTransform;
        titleTextRectTransform.sizeDelta = new Vector2(1000, titleTextRectTransform.sizeDelta.y);

        TMP_TextInfo info = textComponent.GetTextInfo(text);

        float bottomRightX = info.characterInfo[0].bottomRight.x;

        for (int i = 1; i < text.Length; i++)
        {
            TMP_CharacterInfo characterInfo = info.characterInfo[i];

            if (char.IsSymbol(characterInfo.character))
                continue;
            if (characterInfo.bottomRight.x >= bottomRightX)
                bottomRightX = characterInfo.bottomRight.x;
        }
        float width = bottomRightX - info.characterInfo[0].bottomLeft.x + 0.05f;

        titleTextRectTransform.sizeDelta = new Vector2(width, titleTextRectTransform.sizeDelta.y);
        textComponent.text = text;

        float fullLength = titleTextRectTransform.anchoredPosition.x + width;
        textParentRectTransform.anchoredPosition = new Vector2(fullLength / -2, textParentRectTransform.anchoredPosition.y);
    }
}
