using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;
using TMPro;


public class DialogueUI : AbstractMenu, IDialogueInterpreter
{
    [SerializeField]
    private SpeakerUIReference mainCharacter;
    [HideInInspector]
    [SerializeField]
    private SpeakerUIReference npc;

    [Header("Button")]
    [SerializeField]
    private GameObject choiceTextPrefab;
    [SerializeField]
    private Vector2 firstAnchoredPosition;
    [SerializeField]
    private Vector2 offset;

    [Header("Event")]
    [SerializeField]
    private EventReference dialogueEndEvent;
    [SerializeField]
    private EventReference dialogueStartEvent;

    [Header("Merchant")]
    [SerializeField]
    [LauguageID]
    private int merchantNoThanksLanguageID;
    [SerializeField]
    private Inventory playerInventory;

    private List<GameObject> _aliveChoices;
    private DialogueGraph _dialogueGraph;
    private Canvas _canvas;

    private void Start()
    {
        _aliveChoices = new List<GameObject>();

        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        mainCharacter.Parent.SetActive(false);
        npc.Parent.SetActive(false);

        dialogueStartEvent.InvokeDialogueGraphEvents += StartDialogue;
    }

    public void StartDialogue(DialogueGraph graph)
    {
        if (_dialogueGraph != null) return;

        OpenMenu();
        _canvas.enabled = true;

        _dialogueGraph = graph;
        _dialogueGraph.SetUp(this);
        _dialogueGraph.Start();
        _dialogueGraph.Proccessing();
        return;
    }


#region Change dialogue UI text
    public void ChangeToQuestion(QuestionNode node)
    {
        CleanUpLastNode();
        mainCharacter.ChangeUI(node.Speaker, node.ContentLaguageID, false);

        Vector2 anchoredPosition = firstAnchoredPosition;
        for (int i = node.choices.Length - 1; i >= 0; i--)
        {
            GameObject newChoiceButton = Instantiate(choiceTextPrefab, choiceTextPrefab.transform.parent);
            newChoiceButton.SetActive(true);

            var transform = newChoiceButton.GetComponent<RectTransform>();
            transform.anchoredPosition = anchoredPosition;
            anchoredPosition += offset;

            var languageText = newChoiceButton.GetComponentInChildren<LanguageText>();
            languageText.ChangeId(node.choices[i].ContentLaguageID);

            var button = newChoiceButton.GetComponent<Button>();
            int index = i;
            button.onClick.AddListener(delegate
            {
                ChoiceButtonClicked(node, index);
            });

            _aliveChoices.Add(newChoiceButton);
        }
    }

    public void ChangeToDialogue(DialogueNode node)
    {
        CleanUpLastNode();
        mainCharacter.ChangeUI(node.Speaker, node.ContentLaguageID, true);
    }

    public void ChangeToMerchant(OpenMerchantNode node)
    {
        CleanUpLastNode();
        mainCharacter.ChangeUI(node.Speaker, node.ContentLaguageID, false);


        GameObject newChoiceButton = Instantiate(choiceTextPrefab, choiceTextPrefab.transform.parent);
        newChoiceButton.SetActive(true);

        newChoiceButton.GetComponent<RectTransform>().anchoredPosition = firstAnchoredPosition;
        newChoiceButton.GetComponentInChildren<LanguageText>().ChangeId(merchantNoThanksLanguageID);
        newChoiceButton.GetComponent<Button>().onClick.AddListener(delegate { ChoiceButtonClicked(node, -1); });

        _aliveChoices.Add(newChoiceButton);


        Merchant merchant = node.Merchant;

        Vector2 anchoredPosition = firstAnchoredPosition + offset;
        for (int i = merchant.Merchandises.Length - 1; i >= 0; i--)
        {
            Merchant.Merchandise merchandise = merchant.Merchandises[i];

            if (merchant.CheckMerchandiseLimitReached(i)) continue;

            InstantiateMerchantButton(node, merchandise, anchoredPosition, i);
            anchoredPosition += offset;
        }
    }

    private void InstantiateMerchantButton(OpenMerchantNode node, Merchant.Merchandise merchandise, Vector2 anchoredPosition, int i)
    {
        GameObject newChoiceButton = Instantiate(choiceTextPrefab, choiceTextPrefab.transform.parent);
        newChoiceButton.SetActive(true);

        newChoiceButton.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        newChoiceButton.GetComponentInChildren<LanguageText>().ChangeId(merchandise.NameLanguageID);

        var button = newChoiceButton.GetComponent<Button>();
        button.interactable = playerInventory.CoreCount >= merchandise.RequireCoreCount;
        button.onClick.AddListener(delegate { ChoiceButtonClicked(node, i); });

        _aliveChoices.Add(newChoiceButton);
    }
    #endregion


    public void OnDialogueEnd()
    {
        _dialogueGraph.TearDown();
        _dialogueGraph = null;
        _canvas.enabled = false;
        dialogueEndEvent?.Invoke();
        CloseMenu();
    }

    private void CleanUpLastNode()
    {
        for (int i = 0; i < _aliveChoices.Count; i++)
        {
            Destroy(_aliveChoices[i]);
        }
    }

    public void ChoiceButtonClicked(QuestionNode node, int index)
    {
        node.MakeChoice(index);
        _dialogueGraph.Proccessing();
    }

    public void ChoiceButtonClicked(OpenMerchantNode node, int index)
    {
        if (index == -1)
        {
            node.Skip();
        }
        else
        {
            Merchant.Merchandise merchandise = node.Merchant.Merchandises[index];
            node.Merchant.BuyCount[index]++;
            playerInventory.ChangeCoreCount(-merchandise.RequireCoreCount);

            if (merchandise.BowUpgrade)
            {
                GameObject.FindWithTag(PlayerBehaviour.Tag).GetComponent<PlayerBehaviour>().UpgradeBow(merchandise.BowUpgrade);
                GameObject.Find("HUD").GetComponent<PlayerStatusHUD>().UnlockBowUpgrade(merchandise.BowUpgrade);
            }
            else
            {
                playerInventory.ChangeAppleCount(merchandise.AppleGain);
            }
            node.Skip();
        }
        _dialogueGraph.Proccessing();
    }

    public void NextDialogue()
    {
        _dialogueGraph.Proccessing();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        dialogueStartEvent.InvokeDialogueGraphEvents -= StartDialogue;
    }

    [System.Serializable]
    public class SpeakerUIReference
    {
        public GameObject Parent;
        public Image CharacterImage;
        public Image NameLabelImage;
        public LanguageText NameLanguageText;
        public LanguageText DialogueLanguageText;
        public Image NextDialogueButton;

        [Header("NPC")]
        public Sprite NpcNameLabel;
        public Sprite NpcNameNext;
        [Header("Main Character")]
        public Sprite MainCharacterNameLabel;
        public Sprite MainCharacterNext;

        public void ChangeUI(Speaker speaker, int dialogueLanguageID, bool showNextButton)
        {
            CharacterImage.sprite = speaker.CharacterSprite;
            NameLanguageText.ChangeId(speaker.NameLanguageID);
            DialogueLanguageText.ChangeId(dialogueLanguageID);

            NextDialogueButton.sprite = speaker.IsNPC ? NpcNameNext : MainCharacterNext;
            NextDialogueButton.gameObject.SetActive(showNextButton);

            NameLabelImage.sprite = speaker.IsNPC ? NpcNameLabel : MainCharacterNameLabel;

            Parent.SetActive(true);
        }

        public void Hide()
        {
            Parent.SetActive(false);
        }
    }
}