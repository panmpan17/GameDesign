using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class PositionTween : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private Vector3 endPosition;
    [SerializeField]
    private Timer timer;

    void Update()
    {
        transform.localPosition = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(timer.Progress));
        if (timer.UpdateEnd)
        {
            timer.ReverseMode = !timer.ReverseMode;
        }
    }
}
