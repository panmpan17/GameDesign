using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class PlayerBehaviour : MonoBehaviour
{
    public const string Tag = "Player";

    [Header("Other components")]
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private PlayerInput input;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private new PlayerAnimation animation;
    public PlayerInput Input => input;
    public PlayerMovement Movement => movement;

    [Header("Paramater")]
    [SerializeField]
    private LayerMask hitLayers;
    [SerializeField]
    private float maxHealth;
    private float _health;
    [SerializeField]
    private EventReference healthChangeEvent;

    [Header("Arrows")]
    [SerializeField]
    private Transform arrowPlacePoint;
    [SerializeField]
    private Arrow arrowPrefab;
    [SerializeField]
    private LimitedPrefabPool<Arrow> arrowPrefabPool;

    [SerializeField]
    private Transform aimBall;

    public event System.Action OnDrawBow;
    public event System.Action OnDrawBowEnd;

    public bool CursorFocued { get; protected set; }

    private int _walkingCameraIndex;
    private int _aimCameraIndex;

    public bool IsDrawingBow { get; private set; }
    public Arrow PreparedArrow { get; private set; }

    private Ray _currentRay;
    public Vector3 CurrentRayHitPosition { get; private set; }

    void Awake()
    {
        input.OnAimDown += OnAimDown;
        input.OnAimUp += OnAimUp;

        movement.OnRoll += OnRoll;

        input.OnOutFocus += OnOutFocus;

        _walkingCameraIndex = CameraSwitcher.GetCameraIndex("Walking");
        _aimCameraIndex = CameraSwitcher.GetCameraIndex("Aim");

        arrowPrefabPool = new LimitedPrefabPool<Arrow>(arrowPrefab, 10, true, "Arrows (Pool)");
    }

    void Start()
    {
        _health = maxHealth;
        healthChangeEvent?.Invoke(1);
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

        aimBall.position = CurrentRayHitPosition;
    }


    void OnAimDown()
    {
        if (!CursorFocued)
        {
            CursorFocued = true;
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }

        IsDrawingBow = true;

        if (PreparedArrow == null)
        {
            PreparedArrow = arrowPrefabPool.Get();
            Transform arrowTransform =  PreparedArrow.transform;
            arrowTransform.SetParent(arrowPlacePoint);
            arrowTransform.localPosition = Vector3.zero;
            arrowTransform.localRotation = Quaternion.identity;
        }
        else
            PreparedArrow.gameObject.SetActive(true);

        CameraSwitcher.ins.SwitchTo(_aimCameraIndex);
        OnDrawBow?.Invoke();
    }

    void OnAimUp()
    {
        if (!CursorFocued || !IsDrawingBow)
            return;

        IsDrawingBow = false;

        if (animation.IsDrawArrowFullyPlayed)
        {
            PreparedArrow.transform.SetParent(null);
            PreparedArrow.Shoot(CurrentRayHitPosition);
            PreparedArrow = null;
        }
        else
        {
            PreparedArrow.gameObject.SetActive(false);
        }

        CameraSwitcher.ins.SwitchTo(_walkingCameraIndex);
        OnDrawBowEnd?.Invoke();
    }

    void OnOutFocus()
    {
        CursorFocued = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnRoll()
    {
        if (IsDrawingBow)
        {
            IsDrawingBow = false;
            PreparedArrow.gameObject.SetActive(false);
            CameraSwitcher.ins.SwitchTo(_walkingCameraIndex);
            OnDrawBowEnd?.Invoke();
        }
    }

    public void OnDamage(float amount)
    {
        _health -= amount;

        healthChangeEvent?.Invoke(Mathf.Clamp(_health / maxHealth, 0, 1));
    }
}
