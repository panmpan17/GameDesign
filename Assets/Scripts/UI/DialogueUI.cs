using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MPack;
using TMPro;


public class DialogueUI : AbstractMenu, IDialogueInterpreter
{
    [SerializeField]
    [UnityEngine.Serialization.FormerlySerializedAs("mainCharacter")]
    private SpeakerUIReference speakerUI;

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
        speakerUI.Parent.SetActive(false);

        dialogueStartEvent.InvokeDialogueGraphEvents += StartDialogue;
    }

    public void StartDialogue(DialogueGraph graph)
    {
        if (_dialogueGraph != null) return;

        PauseTimeTrack.Pause(this);
        OpenMenu();
        _canvas.enabled = true;

        _dialogueGraph = graph;
        _dialogueGraph.SetUp(this);
        _dialogueGraph.Start();
        _dialogueGraph.Proccessing();
    }


#region Change dialogue UI text
    public void ChangeToQuestion(QuestionNode node)
    {
        CleanUpLastNode();
        speakerUI.ChangeUI(node.Speaker, node.ContentLaguageID, false);

        Vector2 anchoredPosition = firstAnchoredPosition;
        Button lastButton = null;
        for (int i = node.choices.Length - 1; i >= 0; i--)
        {
            Button newButton = InstantiateQuestionButton(node, anchoredPosition, i);
            anchoredPosition += offset;

            if (lastButton)
            {
                Navigation navigation = newButton.navigation;
                navigation.selectOnDown = lastButton;
                newButton.navigation = navigation;

                navigation = lastButton.navigation;
                navigation.selectOnUp = newButton;
                lastButton.navigation = navigation;
            }

            lastButton = newButton;
        }

        EventSystem.current.SetSelectedGameObject(_aliveChoices[_aliveChoices.Count - 1]);
    }

    Button InstantiateQuestionButton(QuestionNode node, Vector2 anchoredPosition, int i)
    {
        GameObject newChoiceButton = Instantiate(choiceTextPrefab, choiceTextPrefab.transform.parent);
        newChoiceButton.SetActive(true);

        var transform = newChoiceButton.GetComponent<RectTransform>();
        transform.anchoredPosition = anchoredPosition;

        var languageText = newChoiceButton.GetComponentInChildren<LanguageText>();
        languageText.ChangeId(node.choices[i].ContentLaguageID);

        var button = newChoiceButton.GetComponent<Button>();
        int index = i;
        button.onClick.AddListener(delegate
        {
            ChoiceButtonClicked(node, index);
        });

        _aliveChoices.Add(newChoiceButton);

        return button;
    }

    public void ChangeToDialogue(DialogueNode node)
    {
        CleanUpLastNode();
        speakerUI.ChangeUI(node.Speaker, node.ContentLaguageID, true);
    }

    public void ChangeToMerchant(OpenMerchantNode node)
    {
        CleanUpLastNode();
        speakerUI.ChangeUI(node.Speaker, node.ContentLaguageID, false);

        Button lastButton = InstantiateMerchantRejectButton(node);

        Merchant merchant = node.Merchant;

        Vector2 anchoredPosition = firstAnchoredPosition + offset;
        for (int i = merchant.Merchandises.Length - 1; i >= 0; i--)
        {
            Merchant.Merchandise merchandise = merchant.Merchandises[i];

            if (merchant.CheckMerchandiseLimitReached(i)) continue;

            Button newButton = InstantiateMerchantButton(node, merchandise, anchoredPosition, i);

            Navigation navigation = newButton.navigation;
            navigation.selectOnDown = lastButton;
            newButton.navigation = navigation;

            navigation = lastButton.navigation;
            navigation.selectOnUp = newButton;
            lastButton.navigation = navigation;

            lastButton = newButton;

            anchoredPosition += offset;
        }

        EventSystem.current.SetSelectedGameObject(_aliveChoices[_aliveChoices.Count - 1]);
    }

    Button InstantiateMerchantRejectButton(OpenMerchantNode node)
    {
        GameObject newChoiceButton = Instantiate(choiceTextPrefab, choiceTextPrefab.transform.parent);
        newChoiceButton.SetActive(true);

        newChoiceButton.GetComponent<RectTransform>().anchoredPosition = firstAnchoredPosition;
        newChoiceButton.GetComponentInChildren<LanguageText>().ChangeId(merchantNoThanksLanguageID);
        var button = newChoiceButton.GetComponent<Button>();
        button.onClick.AddListener(delegate { ChoiceButtonClicked(node, -1); });

        _aliveChoices.Add(newChoiceButton);
        return button;
    }

    Button InstantiateMerchantButton(OpenMerchantNode node, Merchant.Merchandise merchandise, Vector2 anchoredPosition, int i)
    {
        GameObject newChoiceButton = Instantiate(choiceTextPrefab, choiceTextPrefab.transform.parent);
        newChoiceButton.SetActive(true);

        newChoiceButton.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        var languageText = newChoiceButton.GetComponentInChildren<LanguageText>();
        languageText.languageProcess += (text, textMeshPro, textMeshProUGUI) => 
            string.Format("{0}: {1}<sprite=0>", text, merchandise.RequireCoreCount);
        languageText.ChangeId(merchandise.NameLanguageID);

        // : 3 < sprite = 0 >

        var button = newChoiceButton.GetComponent<Button>();
        button.interactable = playerInventory.CoreCount >= merchandise.RequireCoreCount;
        button.onClick.AddListener(delegate { ChoiceButtonClicked(node, i); });

        _aliveChoices.Add(newChoiceButton);

        return button;
    }
    #endregion


    public void OnDialogueEnd()
    {
        PauseTimeTrack.Unpause(this);
        _dialogueGraph.TearDown();
        _dialogueGraph = null;
        _canvas.enabled = false;
        dialogueEndEvent?.Invoke();

        EventSystem.current.SetSelectedGameObject(null);
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
                InventoryGainUI.ins.ShowBowUpgrade(merchandise.BowUpgrade);
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

            NextDialogueButton.gameObject.SetActive(showNextButton);
            if (showNextButton)
            {
                NextDialogueButton.sprite = speaker.IsNPC ? NpcNameNext : MainCharacterNext;
                EventSystem.current.SetSelectedGameObject(NextDialogueButton.gameObject);
            }

            NameLabelImage.sprite = speaker.IsNPC ? NpcNameLabel : MainCharacterNameLabel;

            Parent.SetActive(true);
        }

        public void Hide()
        {
            Parent.SetActive(false);
        }
    }
}