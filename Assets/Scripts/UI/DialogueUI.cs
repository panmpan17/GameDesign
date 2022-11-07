using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;
using TMPro;


public class DialogueUI : AbstractMenu, IDialogueInterpreter
{
    [SerializeField]
    private TextMeshProUGUI questionText;
    [SerializeField]
    private GameObject choiceTextPrefab;
    [SerializeField]
    private GameObject nextDialogue;
    [SerializeField]
    private Vector2 offset;
    [SerializeField]
    private EventReference dialogueEndEvent;
    [SerializeField]
    private EventReference dialogueStartEvent;

    private List<GameObject> _aliveChoices;
    private VaribleStorageSystem _varibleStorageSystem;
    private DialogueGraph _dialogueGraph;
    private Canvas _canvas;

    private void Start()
    {
        _aliveChoices = new List<GameObject>();
        _varibleStorageSystem = new VaribleStorageSystem();

        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;

        dialogueStartEvent.InvokeDialogueGraphEvents += StartDialogue;
    }

    public void StartDialogue(DialogueGraph graph)
    {
        if (_dialogueGraph != null) return;

        OpenMenu();
        _canvas.enabled = true;

        _dialogueGraph = graph;
        _dialogueGraph.SetUp(this, _varibleStorageSystem);
        _dialogueGraph.Start();
        _dialogueGraph.Proccessing();
        return;
    }

    public void ChangeToQuestion(QuestionNode node)
    {
        CleanUpLastNode();

        questionText.text = node.content;

        for (int i = 0; i < node.choices.Length; i++)
        {
            GameObject newChoiceButton = Instantiate(choiceTextPrefab, choiceTextPrefab.transform.parent);
            newChoiceButton.SetActive(true);

            var transform = newChoiceButton.GetComponent<RectTransform>();
            transform.anchoredPosition += offset * i;

            var uiText = newChoiceButton.GetComponentInChildren<TextMeshProUGUI>();
            uiText.text = node.choices[i].content;

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

        questionText.text = node.content;
        nextDialogue.SetActive(true);
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
        nextDialogue.SetActive(false);
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
}