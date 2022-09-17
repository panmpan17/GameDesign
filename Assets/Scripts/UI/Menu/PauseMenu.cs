using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
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
    }

    public void Resume()
    {
        Debug.Log("resume");
        Time.timeScale = 1;
        canvas.enabled = false;
        focusEvent.Invoke();
    }
}
