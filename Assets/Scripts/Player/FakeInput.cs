using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeInput : MonoBehaviour, InputInterface
{
    [SerializeField]
    private bool repeat;
    [SerializeField]
    private InputStep[] steps;
    private int stepIndex;


    public event System.Action OnAimDown;
    public event System.Action OnAimUp;

    public event System.Action OnJump;
    public event System.Action OnJumpEnd;

    public event System.Action OnEscap;

    public event System.Action OnRoll;

    public event System.Action OnInteract;
    public event System.Action OnEatApple;

    public Vector2 MovementAxis { get; private set; }
    public Vector2 LookAxis { get; private set; }
    public bool HasMovementAxis => MovementAxis.sqrMagnitude > 0.01f;
    private float timer;

    public void Enable() => enabled = true;
    public void Disable()
    {
        enabled = false;
        MovementAxis = Vector3.zero;
        LookAxis = Vector3.zero;
    }


    void Start()
    {
        StepStart(steps[0]);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= steps[stepIndex].Duration)
        {
            StepEnd(steps[stepIndex]);

            if (++stepIndex >= steps.Length)
            {
                if (repeat)
                {
                    stepIndex = 0;
                    StepStart(steps[stepIndex]);
                    return;
                }
                enabled = false;
                MovementAxis = Vector2.zero;
            }
            else
            {
                timer = 0;
                StepStart(steps[stepIndex]);
            }
        }
    }

    void StepStart(InputStep step)
    {
        MovementAxis = steps[stepIndex].MovementAxis;
        LookAxis = steps[stepIndex].LookAxis;

        if (steps[stepIndex].Roll)
            OnRoll?.Invoke();

        if (steps[stepIndex].Aim)
            OnAimDown?.Invoke();
    }

    void StepEnd(InputStep step)
    {
        if (steps[stepIndex].Aim)
            OnAimUp?.Invoke();
    }


    [System.Serializable]
    public struct InputStep
    {
        public Vector2 MovementAxis;
        public Vector2 LookAxis;
        public float Duration;
        public bool Roll;
        public bool Aim;
    }
}
