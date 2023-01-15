using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MPack;
using DigitalRuby.Tween;


public class LoadScene : MonoBehaviour
{
    public static LoadScene ins;

    [SerializeField]
    private Transform rect;
    [SerializeField]
    private FillBarControl fillBar;
    private Timer timer = new Timer(10);

    private Canvas _canvas;
    private string _sceneName;
    private AsyncOperation _operation;

    void Awake()
    {
        if (ins)
        {
            Destroy(gameObject);
            return;
        }

        ins = this;
        DontDestroyOnLoad(gameObject);

        _canvas = GetComponent<Canvas>();
        _canvas.enabled = enabled = false;
    }

    public void Update()
    {
        if (_operation == null)
            return;

        fillBar.SetFillAmount(_operation.progress);
    //     if (timer.UpdateEnd)
    //     {
    //         timer.Reset();
    //     }
    //     fillBar.SetFillAmount(timer.Progress);
    }

    public void Load(string sceneName)
    {
        _sceneName = sceneName;
        _canvas.enabled = enabled = true;
        fillBar.SetFillAmount(0);
        gameObject.Tween("LoadRectScale", 0, 1, .8f, TweenScaleFunctions.CubicEaseOut, ScaleRect, ScaleUpCompleted);
    }

    void ScaleRect(ITween<float> data)
    {
        rect.localScale = Vector3.one * data.CurrentValue;
    }

    void ScaleUpCompleted(ITween<float> data)
    {
        _operation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single);

        _operation.completed += delegate {
            gameObject.Tween("LoadRectScale", 1, 0, 0.5f, TweenScaleFunctions.CubicEaseOut, ScaleRect, ScaleDownCompleted);
        };
    }

    void ScaleDownCompleted(ITween<float> data)
    {
        _operation = null;
        _canvas.enabled = enabled = false;

        DroppedItem.Pool.ClearObjects();
    }
}
