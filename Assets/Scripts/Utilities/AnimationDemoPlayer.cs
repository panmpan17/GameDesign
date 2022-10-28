using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class AnimationDemoPlayer : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Timer wait;

    private int index = 0;

    void Awake()
    {
        // animator.runtimeAnimatorController.animationClips;
        animator.Play(animator.runtimeAnimatorController.animationClips[0].name);
        wait.Running = false;
    }

    void Update()
    {
        if (wait.Running)
        {
            if (wait.UpdateEnd)
            {
                wait.Running = false;

                if (++index >= animator.runtimeAnimatorController.animationClips.Length)
                    index = 0;
                animator.Play(animator.runtimeAnimatorController.animationClips[index].name);
            }
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            wait.Reset();
        }
    }
}
