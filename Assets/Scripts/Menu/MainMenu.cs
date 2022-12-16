using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class MainMenu : AbstractMenu
{
    public string GameSceneName;

    private GameObject _lastSelected;
    
#if UNITY_EDITOR
    [SerializeField]
    private bool fireButtonEvent;
#endif

    void Start()
    {
        OpenMenu();
    }

    public void StartGame()
    {
#if UNITY_EDITOR
        if (!fireButtonEvent) return;
#endif

        LoadScene.ins.Load(GameSceneName);
    }

    public void Setting()
    {
#if UNITY_EDITOR
        if (!fireButtonEvent) return;
#endif

        _lastSelected = EventSystem.current.currentSelectedGameObject;
        AbstractMenu.S_OpenMenu("Setting");
    }

    protected override void BackToThisMenu()
    {
        EventSystem.current.SetSelectedGameObject(_lastSelected);
    }

    public void Exit()
    {

#if UNITY_EDITOR
        if (!fireButtonEvent) return;
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
