using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

    [Header("Saving")]
    [SerializeField]
    private SaveDataReference saveDataReference;
    [SerializeField]
    private EventReference extractSaveDataEvent;

    private InputAction _cancelAction;


    void Awake()
    {
        pauseEvent.InvokeEvents += Pause;
        canvas.enabled = false;
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

        if (_cancelAction == null)
            _cancelAction = EventSystem.current.GetComponent<InputSystemUIInputModule>().cancel.action;
        _cancelAction.performed += OnCancel;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        canvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(null);
        focusEvent.Invoke();
        CloseMenu();

        if (_cancelAction == null)
            _cancelAction = EventSystem.current.GetComponent<InputSystemUIInputModule>().cancel.action;
        _cancelAction.performed -= OnCancel;
    }

    public void OpenSetting()
    {
        _lastSelected = EventSystem.current.currentSelectedGameObject;
        AbstractMenu.S_OpenMenu("Setting");

        _cancelAction.performed -= OnCancel;
    }

    protected override void BackToThisMenu()
    {
        EventSystem.current.SetSelectedGameObject(_lastSelected);
        _cancelAction.performed += OnCancel;
    }

    public void MainMenu()
    {
        extractSaveDataEvent.Invoke();
        saveDataReference.SaveToFilePath(Path.Join(Application.persistentDataPath, "save1"));

        LoadScene.ins.Load("MainMenu");
        Time.timeScale = 1;
    }

    void OnCancel(CallbackContext callbackContext)
    {
        Resume();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        pauseEvent.InvokeEvents -= Pause;
        Time.timeScale = 1;

        // somehow, even when ui input module destroy, the action won't destroy
        if (_cancelAction != null)
            _cancelAction.performed -= OnCancel;
    }
}
