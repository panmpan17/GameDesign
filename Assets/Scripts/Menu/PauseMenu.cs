using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;
using UnityEngine.InputSystem.UI;


public class PauseMenu : AbstractMenu
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject firstSelected;
    private GameObject _lastSelected;

    [SerializeField]
    private EventReference pauseEvent;
    [SerializeField]
    private EventReference focusEvent;

    private InputSystemUIInputModule uiInputModule;


    void Awake()
    {
        pauseEvent.InvokeEvents += Pause;
        canvas.enabled = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        pauseEvent.InvokeEvents -= Pause;
        Time.timeScale = 1;
    }

    void Pause()
    {
        Time.timeScale = 0;
        canvas.enabled = true;
        EventSystem.current.SetSelectedGameObject(firstSelected);
        OpenMenu();
        StartCoroutine(C_Delay());
    }

    IEnumerator C_Delay()
    {
        yield return new WaitForEndOfFrame();
        uiInputModule ??= EventSystem.current.GetComponent<InputSystemUIInputModule>();
        uiInputModule.cancel.action.performed += OnCancel;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        canvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(null);
        focusEvent.Invoke();
        CloseMenu();

        var uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
        uiInputModule.cancel.action.performed -= OnCancel;
    }

    public void OpenSetting()
    {
        _lastSelected = EventSystem.current.currentSelectedGameObject;
        AbstractMenu.S_OpenMenu("Setting");

        uiInputModule.cancel.action.performed -= OnCancel;
    }

    protected override void BackToThisMenu()
    {
        EventSystem.current.SetSelectedGameObject(_lastSelected);
        uiInputModule.cancel.action.performed += OnCancel;
    }

    public void MainMenu()
    {
        LoadScene.ins.Load("MainMenu");
        Time.timeScale = 1;
    }

    void OnCancel(CallbackContext callbackContext)
    {
        Resume();
    }
}
