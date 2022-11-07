using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveBase : MonoBehaviour
{
    public const string Tag = "Interactive";

    public UnityEvent OnArrowHit;

    public void ArrowHit()
    {
        OnArrowHit.Invoke();
    }
}
