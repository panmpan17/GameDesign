using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;


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

    private InputSystemUIInputModule _uiInputModule;


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

        if (_uiInputModule)
            _uiInputModule.cancel.action.performed -= OnCancel;
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
        _uiInputModule ??= EventSystem.current.GetComponent<InputSystemUIInputModule>();
        _uiInputModule.cancel.action.performed += OnCancel;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        canvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(null);
        focusEvent.Invoke();
        CloseMenu();

        _uiInputModule ??= EventSystem.current.GetComponent<InputSystemUIInputModule>();
        _uiInputModule.cancel.action.performed -= OnCancel;
    }

    public void OpenSetting()
    {
        _lastSelected = EventSystem.current.currentSelectedGameObject;
        AbstractMenu.S_OpenMenu("Setting");

        _uiInputModule.cancel.action.performed -= OnCancel;
    }

    protected override void BackToThisMenu()
    {
        EventSystem.current.SetSelectedGameObject(_lastSelected);
        _uiInputModule.cancel.action.performed += OnCancel;
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
