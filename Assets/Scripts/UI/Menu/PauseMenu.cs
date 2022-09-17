using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MPack;


public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject firstSelected;
    private GameObject _lastSelected;
    [SerializeField]
    private SettingMenu settingMenu;

    [SerializeField]
    private EventReference pauseEvent;
    [SerializeField]
    private EventReference focusEvent;


    void Awake()
    {
        pauseEvent.InvokeEvents += Pause;
        canvas.enabled = false;
    }

    void OnDistroy()
    {
        pauseEvent.InvokeEvents -= Pause;
        Time.timeScale = 1;
    }

    void Pause()
    {
        Time.timeScale = 0;
        canvas.enabled = true;
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        canvas.enabled = false;
        focusEvent.Invoke();
    }

    public void OpenSetting()
    {
        _lastSelected = EventSystem.current.currentSelectedGameObject;
        settingMenu.Activate();
    }
}
