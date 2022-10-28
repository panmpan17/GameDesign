using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class AnimationRandomPlayer : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private RangeReference timerRange;
    private Timer _timer;
    [SerializeField]
    private int layer;
    [SerializeField]
    private string[] animationNames;
    private int[] animationHash;

    void Start()
    {
        animationHash = new int[animationNames.Length];
        for (int i = 0; i < animationNames.Length; i++)
        {
            animationHash[i] = Animator.StringToHash(animationNames[i]);
        }

        _timer = new Timer(timerRange.PickRandomNumber());
    }

    void Update()
    {
        if (_timer.UpdateEnd)
        {
            _timer.Reset();
            _timer.TargetTime = timerRange.PickRandomNumber();

            animator.Play(animationHash[Random.Range(0, animationHash.Length)], layer);
        }
    }
}
