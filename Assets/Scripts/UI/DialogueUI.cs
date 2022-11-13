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
    [SerializeField]
    private SpeakerUIReference npc;

    [SerializeField]
    private GameObject choiceTextPrefab;
    [SerializeField]
    private Vector2 offset;
    [SerializeField]
    private EventReference dialogueEndEvent;
    [SerializeField]
    private EventReference dialogueStartEvent;

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

    public void ChangeToQuestion(QuestionNode node)
    {
        CleanUpLastNode();

        if (node.Speaker.IsNPC)
        {
            npc.ChangeUI(node.Speaker, node.ContentLaguageID, false);
            mainCharacter.Hide();
        }
        else
        {
            mainCharacter.ChangeUI(node.Speaker, node.ContentLaguageID, false);
            npc.Hide();
        }

        for (int i = 0; i < node.choices.Length; i++)
        {
            GameObject newChoiceButton = Instantiate(choiceTextPrefab, choiceTextPrefab.transform.parent);
            newChoiceButton.SetActive(true);

            var transform = newChoiceButton.GetComponent<RectTransform>();
            transform.anchoredPosition += offset * i;

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

        if (node.Speaker.IsNPC)
        {
            npc.ChangeUI(node.Speaker, node.ContentLaguageID, true);
            mainCharacter.Hide();
        }
        else
        {
            mainCharacter.ChangeUI(node.Speaker, node.ContentLaguageID, true);
            npc.Hide();
        }
    }

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
        ((QuestionNode)_dialogueGraph.currentNode).MakeChoice(index);
        _dialogueGraph.Proccessing();
    }

    public void NextDialogue()
    {
        _dialogueGraph.Proccessing();
    }

    [System.Serializable]
    public class SpeakerUIReference
    {
        public GameObject Parent;
        public Image CharacterImage;
        public LanguageText NameLanguageText;
        public LanguageText DialogueLanguageText;
        public GameObject NextDialogueButton;

        public void ChangeUI(Speaker speaker, int dialogueLanguageID, bool showNextButton)
        {
            CharacterImage.sprite = speaker.CharacterSprite;
            NameLanguageText.ChangeId(speaker.NameLanguageID);
            DialogueLanguageText.ChangeId(dialogueLanguageID);
            NextDialogueButton.SetActive(showNextButton);
            Parent.SetActive(true);
        }

        public void Hide()
        {
            Parent.SetActive(false);
        }
    }
}