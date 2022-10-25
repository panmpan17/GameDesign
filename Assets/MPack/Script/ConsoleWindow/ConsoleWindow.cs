using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace MPack
{
    public class ConsoleWindow : MonoBehaviour
    {
        public const string PrefabPathInResources = "ConsoleWindow";

        private static bool loadingPrefab = false;

        public static void ToggleConsoleWindow()
        {
            if (loadingPrefab) return;

            ConsoleWindow window = FindObjectOfType<ConsoleWindow>(true);
            if (window == null)
            {
                loadingPrefab = true;

                ResourceRequest request = Resources.LoadAsync<ConsoleWindow>(PrefabPathInResources);
                request.completed += delegate {
                    window = Instantiate((ConsoleWindow)request.asset);
                    loadingPrefab = false;
                };
            }
            else    
            {
                window.gameObject.SetActive(!window.gameObject.activeSelf);
            }
        }

        public ImproveInputField commandInput;

        public Color errorCommandColor;

        public RectTransform suggestionsRectTransform;

        public int suggestionsLength;
        private Text[] suggestionTexts;

        private AttributeMethodFetch commandFetcher;

        private void Start() {
            commandFetcher = new AttributeMethodFetch();
            commandFetcher.ReindexCommandsInAllAssemblies();

            suggestionTexts = new Text[suggestionsLength];
            suggestionTexts[0] = suggestionsRectTransform.GetComponentInChildren<Text>();
            SimpleSciptableButton button = suggestionTexts[0].GetComponent<SimpleSciptableButton>();
            button.identifiedIndex = 0;
            button.onClick += CommandButtonClicked;

            for (int i = 1; i < suggestionsLength; i++)
            {
                suggestionTexts[i] = Instantiate(suggestionTexts[0], suggestionTexts[0].transform.parent);
                suggestionTexts[i].rectTransform.anchoredPosition += new Vector2(0, -30 * i);
                suggestionTexts[i].text = i.ToString();

                button = suggestionTexts[i].GetComponent<SimpleSciptableButton>();
                button.identifiedIndex = i;
                button.onClick += CommandButtonClicked;
            }

            suggestionsRectTransform.gameObject.SetActive(false);
            commandInput.onClick += OnCommandInputFieldSelect;
            commandInput.onDeselect += OnCommandInputFieldDeselect;
            commandInput.onValueChanged.AddListener(CommandInputFieldChanged);


            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
            // bool enterPressed = Input.GetKeyDown(KeyCode.Return);
            bool enterPressed = Keyboard.current.enterKey.wasPressedThisFrame;

            if (enterPressed && EventSystem.current.currentSelectedGameObject == commandInput.gameObject)
            {
                InputCommand(commandInput.text);

                commandInput.text = "";
                OnCommandInputFieldDeselect();

                EventSystem.current.SetSelectedGameObject(null);
                return;
            }

            if (Keyboard.current.slashKey.wasPressedThisFrame)
                EventSystem.current.SetSelectedGameObject(commandInput.gameObject);
        }

        private void InputCommand(string command)
        {
            CommandMethod commandMethod;
            object[] arguments;
            if (commandFetcher.FindCommandMatch(command, out commandMethod, out arguments))
            {
                commandMethod.method.Invoke(null, arguments);
            }
        }

        private void ReloadCommandSuggestions(CommandMethod[] commandMethods)
        {
            int avalibleLength = 0;
            for (int i = 0; i < suggestionsLength; i++)
            {
                if (commandMethods[i] == null)
                    suggestionTexts[i].gameObject.SetActive(false);
                else
                {
                    suggestionTexts[i].gameObject.SetActive(true);
                    suggestionTexts[i].text = commandMethods[i].FormmatedSuggestion();
                    avalibleLength++;
                }
            }
            suggestionsRectTransform.sizeDelta = new Vector2(suggestionsRectTransform.sizeDelta.x, 30 * avalibleLength);

            // Some indicator that commands is wrong
            commandInput.textComponent.color = avalibleLength == 0? errorCommandColor: Color.white;
        }

        #region UI Element Event
        private void OnCommandInputFieldSelect()
        {
            if (commandInput.text == "")
                ReloadCommandSuggestions(commandFetcher.GetCommandSuggestions(suggestionsLength));
            else
                ReloadCommandSuggestions(commandFetcher.GetCommandSuggestions(commandInput.text, suggestionsLength));
            suggestionsRectTransform.gameObject.SetActive(true);
        }

        private void OnCommandInputFieldDeselect()
        {
            StartCoroutine(ExecuteAfterFrame(delegate {
                suggestionsRectTransform.gameObject.SetActive(false);
            }));
        }

        private void CommandInputFieldChanged(string commandValue)
        {
            ReloadCommandSuggestions(commandFetcher.GetCommandSuggestions(commandValue, suggestionsLength));
        }

        private void CommandButtonClicked(SimpleSciptableButton button, PointerEventData eventData)
        {
            CommandMethod commandMethod = commandFetcher.commandSuggestionsCache[button.identifiedIndex];

            commandInput.text = commandMethod.PureTextSuggestion();
            // ReloadCommandSuggestions(commandFetcher.GetCommandSuggestions(commandInput.text, suggestionsLength));
            commandInput.OnPointerClick(new PointerEventData(eventSystem: EventSystem.current));
            StartCoroutine(ExecuteAfterFrame(delegate
            {
                suggestionsRectTransform.gameObject.SetActive(true);
                commandInput.MoveTextEnd(false);
            }));

            // string commandText = button.GetComponent<Text>().text;
            // InputCommand(commandText);

            // commandInput.text = "";
            // OnCommandInputFieldDeselect();
        }
        #endregion


        private IEnumerator ExecuteAfterFrame(System.Action action)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            action.Invoke();
        }
    }
}