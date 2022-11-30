using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using MPack;


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
    }

    public void Resume()
    {
        Time.timeScale = 1;
        canvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(null);
        focusEvent.Invoke();
        CloseMenu();
    }

    public void OpenSetting()
    {
        _lastSelected = EventSystem.current.currentSelectedGameObject;
        AbstractMenu.S_OpenMenu("Setting");
    }

    protected override void BackToThisMenu()
    {
        EventSystem.current.SetSelectedGameObject(_lastSelected);
    }

    public void MainMenu()
    {
        LoadScene.ins.Load("MainMenu");
        Time.timeScale = 1;
    }
}
