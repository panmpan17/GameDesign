using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using Cinemachine;

public class PlayerBehaviour : MonoBehaviour
{
    public const string Tag = "Player";

    [Header("Other components")]
    // [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private InputInterface input;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private new PlayerAnimation animation;
    public InputInterface Input => input;
    public PlayerMovement Movement => movement;

    [Header("Health")]
    [SerializeField]
    private float maxHealth;
    private float _health;
    [SerializeField]
    private EventReference healthChangeEvent;

    [Header("Aim")]
    [SerializeField]
    private Transform arrowPlacePoint;
    [SerializeField]
    private Arrow arrowPrefab;
    [SerializeField]
    private LimitedPrefabPool<Arrow> arrowPrefabPool;
    [SerializeField]
    private CinemachineImpulseSource impulseSource;

    [Header("Others")]
    [SerializeField]
    private LayerMask hitLayers;
    [SerializeField]
    private EventReference pauseEvent;
    [SerializeField]
    private EventReference focusEvent;
    [SerializeField]
    private TransformPointer transformPointer;
    
    [Header("Inventory")]
    [SerializeField]
    private EventReference inventoryEvent;
    private int _itemCount;

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool focusCursorWhenPointerDown;
#endif

    public event System.Action OnDrawBow;
    public event System.Action OnDrawBowEnd;
    public event System.Action OnBowShoot;
    public event System.Action OnDeath;
    public event System.Action OnRevive;

    public bool CursorFocued { get; protected set; }
    public bool IsDrawingBow { get; private set; }
    public bool IsDead => _handleDeath;
    public bool CanDamage => !Movement.IsRolling && !_handleDeath;
    public Arrow PreparedArrow { get; private set; }

    private int _walkingCameraIndex;
    private int _aimCameraIndex;

    private bool _handleDeath = false;

    private Ray _currentRay;
    public Vector3 CurrentRayHitPosition { get; private set; }


    void Awake()
    {
        input = GetComponent<InputInterface>();
        input.OnAimDown += OnAimDown;
        input.OnAimUp += OnAimUp;
        input.OnEscap += OnEscap;

        movement.OnRollEvent += OnRoll;

        focusEvent.InvokeEvents += FocusCursor;

        _walkingCameraIndex = CameraSwitcher.GetCameraIndex("Walk");
        _aimCameraIndex = CameraSwitcher.GetCameraIndex("Aim");

        arrowPrefabPool = new LimitedPrefabPool<Arrow>(arrowPrefab, 10, true, "Arrows (Pool)");

        mainCamera = mainCamera == null ? Camera.main : mainCamera;
        transformPointer.Target = transform;
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
#if UNITY_EDITOR
            if (focusCursorWhenPointerDown)
                FocusCursor();
#endif
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

        CameraSwitcher.ins.SwitchTo(_walkingCameraIndex);
        OnDrawBowEnd?.Invoke();

        if (animation.IsDrawArrowFullyPlayed)
        {
            PreparedArrow.transform.SetParent(null);
            PreparedArrow.Shoot(CurrentRayHitPosition);
            PreparedArrow = null;
            OnBowShoot?.Invoke();
            impulseSource.GenerateImpulse();
        }
        else
        {
            PreparedArrow.gameObject.SetActive(false);
        }
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
        if (!CanDamage) return;

        _health -= amount;

        healthChangeEvent?.Invoke(Mathf.Clamp(_health / maxHealth, 0, 1));

        if (_health <= 0)
        {
            _handleDeath = true;
            OnDeath?.Invoke();
        }
    }

    public void ReviveAtSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        _handleDeath = false;

        _health = maxHealth;
        healthChangeEvent?.Invoke(1f);

        movement.CharacterController.enabled = false;
        transform.position = spawnPoint.transform.position;
        movement.CharacterController.enabled = true;

        CameraSwitcher.ins.SwitchTo(_walkingCameraIndex);
        CameraSwitcher.GetCamera(_walkingCameraIndex).CancelDamping();

        OnRevive?.Invoke();
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
