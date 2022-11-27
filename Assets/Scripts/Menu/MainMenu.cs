using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public string GameSceneName;
    
#if UNITY_EDITOR
    [SerializeField]
    private bool fireButtonEvent;
#endif

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

        AbstractMenu.S_OpenMenu("Setting");
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
