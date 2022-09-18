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

    [Header("Health")]
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

    [Header("Others")]
    [SerializeField]
    private LayerMask hitLayers;
    [SerializeField]
    private EventReference pauseEvent;
    [SerializeField]
    private EventReference focusEvent;
    
    [Header("Inventory")]
    [SerializeField]
    private EventReference inventoryEvent;
    private int _itemCount;

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

        input.OnEscap += OnEscap;

        focusEvent.InvokeEvents += FocusCursor;

        _walkingCameraIndex = CameraSwitcher.GetCameraIndex("Walk");
        _aimCameraIndex = CameraSwitcher.GetCameraIndex("Aim");

        arrowPrefabPool = new LimitedPrefabPool<Arrow>(arrowPrefab, 10, true, "Arrows (Pool)");
    }

    void Start()
    {
        _health = maxHealth;
        healthChangeEvent?.Invoke(1);

        FocusCursor();
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
            // FocusCursor();
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

    void OnEscap()
    {
        CursorFocued = false;
        Cursor.lockState = CursorLockMode.None;

        pauseEvent.Invoke();
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

    public void FocusCursor()
    {
        CursorFocued = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void AddItem(ItemType itemType)
    {
        _itemCount += 1;
        inventoryEvent.Invoke(_itemCount);
    }

    void OnDestroy()
    {
        focusEvent.InvokeEvents -= FocusCursor;
    }
}
