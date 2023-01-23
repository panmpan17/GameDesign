using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.IO;

public class MainMenu : AbstractMenu
{
    public string GameSceneName;

    [SerializeField]
    private PlayableDirector cutscene;
    [SerializeField]
    private GameObject creditMenu;
    [SerializeField]
    private GameObject creditCloseButton;
    [SerializeField]
    private GameObject skipButton;

    [SerializeField]
    private SaveDataReference saveDataReference;

    private GameObject _lastSelected;
    private Coroutine _delayLoadScene;

    void Start()
    {
        OpenMenu();
    }

    public void StartGame()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<Canvas>().enabled = false;
        cutscene.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(skipButton);
        _delayLoadScene = StartCoroutine(C_DelayLoadScene());
        // cutscene.stopped += OnCutSceneFinished;
    }

    IEnumerator C_DelayLoadScene()
    {
        yield return new WaitForSeconds((float)cutscene.duration);
        _delayLoadScene = null;
        LoadScene.ins.Load(GameSceneName);
    }

    public void OpenSaveMenu()
    {
        saveDataReference.ReadFromFilePath(Path.Join(Application.persistentDataPath, "save1"));

        LoadScene.ins.LoadWithSaveData(GameSceneName);
    }

    public void Setting()
    {
        _lastSelected = EventSystem.current.currentSelectedGameObject;
        AbstractMenu.S_OpenMenu("Setting");
    }

    protected override void BackToThisMenu()
    {
        EventSystem.current.SetSelectedGameObject(_lastSelected);
    }

    public void SkipCutscene()
    {
        if (_delayLoadScene != null)
            StopCoroutine(_delayLoadScene);
        _delayLoadScene = null;

        LoadScene.ins.Load(GameSceneName);
    }

    public void Exit()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OpenCredit()
    {
        creditMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(creditCloseButton);
    }

    public void CloseCredit(GameObject backToButton)
    {
        creditMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(backToButton);
    }
}
