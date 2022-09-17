using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class PlayerInput : MonoBehaviour
{
    private InputScheme _scheme;


    public event System.Action OnAimDown;
    public event System.Action OnAimUp;

    public event System.Action OnJump;

    public event System.Action OnEscap;

    public event System.Action OnRoll;


    public Vector2 MovementAxis { get; private set; }
    public Vector2 LookAxis { get; private set; }

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

        _scheme.Player.Escap.performed += OnOutEscapPerformed;

        _scheme.Player.Roll.performed += OnRollPerformed;
    }

    void OnEnable()
    {
        _scheme.Enable();
    }
    void OnDisable()
    {
        _scheme.Disable();
    }

    void OnMovePerformed(CallbackContext callbackContext) => MovementAxis = callbackContext.ReadValue<Vector2>();
    void OnMoveCanceled(CallbackContext callbackContext) => MovementAxis = callbackContext.ReadValue<Vector2>();

    void OnLookPerformed(CallbackContext callbackContext) => LookAxis = callbackContext.ReadValue<Vector2>();
    void OnLookCanceled(CallbackContext callbackContext) => LookAxis = callbackContext.ReadValue<Vector2>();

    void OnAimPerformed(CallbackContext callbackContext) => OnAimDown?.Invoke();
    void OnAimCanceled(CallbackContext callbackContext) => OnAimUp?.Invoke();

    void OnJumpPerformed(CallbackContext callbackContext) => OnJump?.Invoke();

    void OnOutEscapPerformed(CallbackContext callbackContext) => OnEscap?.Invoke();

    void OnRollPerformed(CallbackContext callbackContext) => OnRoll?.Invoke();
}
