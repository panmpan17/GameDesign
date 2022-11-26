using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnStartEvent : MonoBehaviour
{
    public UnityEvent OnStart;


    public UnityEvent OnStartEndOfFrame;

    [SerializeField]
    private float delay;
    public UnityEvent OnDelayStart;

    IEnumerator Start()
    {
        OnStart.Invoke();
        yield return new WaitForEndOfFrame();
        OnStartEndOfFrame.Invoke();
        yield return new WaitForSeconds(delay);
        OnDelayStart.Invoke();
    }
}
