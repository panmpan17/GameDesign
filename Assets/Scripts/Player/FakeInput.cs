using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeInput : MonoBehaviour, InputInterface
{
    [SerializeField]
    private InputStep[] steps;
    private int stepIndex;


    public event System.Action OnAimDown;
    public event System.Action OnAimUp;

    public event System.Action OnJump;

    public event System.Action OnEscap;

    public event System.Action OnRoll;

    public Vector2 MovementAxis { get; private set; }
    public Vector2 LookAxis { get; private set; }
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
        MovementAxis = steps[stepIndex].MovementAxis;
        LookAxis = steps[stepIndex].LookAxis;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= steps[stepIndex].Duration)
        {
            if (++stepIndex >= steps.Length)
            {
                enabled = false;
                MovementAxis = Vector2.zero;
            }
            else
            {
                timer = 0;
                MovementAxis = steps[stepIndex].MovementAxis;
                LookAxis = steps[stepIndex].LookAxis;

                if (steps[stepIndex].Roll)
                    OnRoll?.Invoke();
            }
        }
    }


    [System.Serializable]
    public struct InputStep
    {
        public Vector2 MovementAxis;
        public Vector2 LookAxis;
        public float Duration;
        public bool Roll;
    }
}
