using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    private EventReference eventReference;

    public void Trigger()
    {
        eventReference.Invoke();
    }
}
