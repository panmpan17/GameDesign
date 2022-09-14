using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private LayerMask hitLayers;

    public event System.Action OnDrawBow;
    public event System.Action OnDrawBowEnd;

    public bool CursorFocued { get; protected set; }

    private int _walkingCameraIndex;
    private int _aimCameraIndex;

    public bool IsDrawingBow { get; private set; }

    private Ray _currentRay;
    public Vector3 CurrentRayHitPosition { get; private set; }

    void Awake()
    {
        playerInput.OnAimDown += OnAimDown;
        playerInput.OnAimUp += OnAimUp;

        playerInput.OnOutFocus += OnOutFocus;

        _walkingCameraIndex = CameraSwitcher.GetCameraIndex("Walking");
        _aimCameraIndex = CameraSwitcher.GetCameraIndex("Aim");
    }


    void Update()
    {
        _currentRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(_currentRay, out RaycastHit hit, 50, hitLayers))
        {
            CurrentRayHitPosition = hit.point;
        }
        else
        {
            CurrentRayHitPosition = _currentRay.GetPoint(50);
        }
    }


    void OnAimDown()
    {
        if (!CursorFocued)
        {
            CursorFocued = true;
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }

        CameraSwitcher.ins.SwitchTo(_aimCameraIndex);
        IsDrawingBow = true;
        OnDrawBow?.Invoke();
    }

    void OnAimUp()
    {
        if (!CursorFocued)
            return;

        CameraSwitcher.ins.SwitchTo(_walkingCameraIndex);
        IsDrawingBow = false;
        OnDrawBowEnd?.Invoke();
    }

    void OnOutFocus()
    {
        CursorFocued = false;
        Cursor.lockState = CursorLockMode.None;
    }
}
