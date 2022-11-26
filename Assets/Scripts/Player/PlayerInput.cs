using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public interface InputInterface
{
    event System.Action OnAimDown;
    event System.Action OnAimUp;

    event System.Action OnJump;
    event System.Action OnJumpEnd;

    event System.Action OnEscap;

    event System.Action OnRoll;

    event System.Action OnInteract;

    Vector2 MovementAxis { get; }
    Vector2 LookAxis { get; }
    bool HasMovementAxis { get; }

    void Enable();
    void Disable();
}


public class PlayerInput : MonoBehaviour, InputInterface
{
    [SerializeField]
    private FloatReference lookAxisXSensitive;
    [SerializeField]
    private FloatReference lookAxisYSensitive;

    [Header("Debug use")]
    [SerializeField]
    private bool allowToggleConsoleWindow;

    private InputScheme _scheme;

    public event System.Action OnAimDown;
    public event System.Action OnAimUp;

    public event System.Action OnJump;
    public event System.Action OnJumpEnd;

    public event System.Action OnEscap;

    public event System.Action OnRoll;

    public event System.Action OnInteract;


    public Vector2 MovementAxis { get; private set; }
    public Vector2 LookAxis { get; private set; }
    public bool HasMovementAxis => MovementAxis.sqrMagnitude > 0.01f;
    // input.MovementAxis.x != 0 || input.MovementAxis.y != 0

    void Awake()
    {
        _scheme = new InputScheme();
        BindScheme();
    }

    void BindScheme()
    {
        _scheme.Player.Move.performed += OnMovePerformed;
        _scheme.Player.Move.canceled += OnMoveCanceled;

        _scheme.Player.Look.performed += OnLookPerformed;
        _scheme.Player.Look.canceled += OnLookCanceled;

        _scheme.Player.Aim.performed += OnAimPerformed;
        _scheme.Player.Aim.canceled += OnAimCanceled;

        _scheme.Player.Jump.performed += OnJumpPerformed;
        _scheme.Player.Jump.canceled += OnJumpCanceled;

        _scheme.Player.Escap.performed += OnOutEscapPerformed;

        _scheme.Player.Roll.performed += OnRollPerformed;

        _scheme.Player.Interact.performed += OnInteractPerformed;

        _scheme.Player.ToggleConsoleWindow.performed += OnToggleConsoleWindow;
    }

    public void Enable() => enabled = true;
    void OnEnable()
    {
        _scheme.Enable();
        MovementAxis = Vector2.zero;
        LookAxis = Vector2.zero;
    }
    public void Disable() => enabled = false;
    void OnDisable()
    {
        _scheme.Disable();
        MovementAxis = Vector2.zero;
        LookAxis = Vector2.zero;
    }

    void OnMovePerformed(CallbackContext callbackContext) => MovementAxis = callbackContext.ReadValue<Vector2>();
    void OnMoveCanceled(CallbackContext callbackContext) => MovementAxis = callbackContext.ReadValue<Vector2>();

    void OnLookPerformed(CallbackContext callbackContext) => LookAxis = ChangeByLookSensitive(callbackContext.ReadValue<Vector2>());
    void OnLookCanceled(CallbackContext callbackContext) => LookAxis = ChangeByLookSensitive(callbackContext.ReadValue<Vector2>());

    Vector2 ChangeByLookSensitive(Vector2 axis)
    {
        axis.x *= lookAxisXSensitive.Value;
        axis.y *= lookAxisYSensitive.Value;
        return axis;
    }

    void OnAimPerformed(CallbackContext callbackContext) => OnAimDown?.Invoke();
    void OnAimCanceled(CallbackContext callbackContext) => OnAimUp?.Invoke();

    void OnJumpPerformed(CallbackContext callbackContext) => OnJump?.Invoke();
    void OnJumpCanceled(CallbackContext callbackContext) => OnJumpEnd?.Invoke();

    void OnOutEscapPerformed(CallbackContext callbackContext) => OnEscap?.Invoke();

    void OnRollPerformed(CallbackContext callbackContext) => OnRoll?.Invoke();

    void OnInteractPerformed(CallbackContext callbackContext) => OnInteract?.Invoke();

    void OnToggleConsoleWindow(CallbackContext callbackContext)
    {
        if (allowToggleConsoleWindow)
            ConsoleWindow.ToggleConsoleWindow();

        if (ConsoleWindow.s_IsActive)
        {
            _scheme.Player.Look.Disable();
            _scheme.Player.Aim.Disable();
            _scheme.Player.Move.Disable();
            _scheme.Player.Run.Disable();
            _scheme.Player.Roll.Disable();
            _scheme.Player.Jump.Disable();
            _scheme.Player.Interact.Disable();
        }
        else
        {
            _scheme.Player.Look.Enable();
            _scheme.Player.Aim.Enable();
            _scheme.Player.Move.Enable();
            _scheme.Player.Run.Enable();
            _scheme.Player.Roll.Enable();
            _scheme.Player.Jump.Enable();
            _scheme.Player.Interact.Enable();
        }
        // ConsoleWindow.s_IsActive;
    }
}
