using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using Cinemachine;

public class PlayerBehaviour : MonoBehaviour
{
    public const string Tag = "Player";

    [Header("Other components")]
    private Camera mainCamera;
    [SerializeField]
    private InputInterface input;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private new PlayerAnimation animation;
    public InputInterface Input => input;
    public PlayerMovement Movement => movement;

    [SerializeField]
    private Bow bow;

    [Header("Health")]
    [SerializeField]
    private float maxHealth;
    private float _health;
    [SerializeField]
    private EventReference healthChangeEvent;

    [Header("Others")]
    [SerializeField]
    private LayerMask hitLayers;
    [SerializeField]
    private EventReference pauseEvent;
    [SerializeField]
    private EventReference focusEvent;
    [SerializeField]
    private TransformPointer transformPointer;
    [SerializeField]
    private Transform trackTarget;
    
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
    public event System.Action<float> OnBowShoot;

    public event System.Action OnDeath;
    public event System.Action OnRevive;

    public bool CursorFocued { get; protected set; }
    public bool IsDrawingBow { get; private set; }
    public bool IsDead => _handleDeath;
    public bool CanDamage => !Movement.IsRolling && !_handleDeath;

    private int _walkingCameraIndex;

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

        mainCamera = mainCamera == null ? Camera.main : mainCamera;
        transformPointer.Target = trackTarget ? trackTarget : transform;

        animation.OnAimAnimatinoChanged += OnAimProgress;
        // aimProgressEvent.InvokeFloatEvents;

        bow.Setup(this);
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

        bow.OnAimDown();
        OnDrawBow?.Invoke();
    }

    void OnAimUp()
    {
        if (!CursorFocued || !IsDrawingBow)
            return;

        IsDrawingBow = false;

        bool isBowFullyDrawed = animation.IsDrawArrowFullyPlayed;
        bow.OnAimUp(isBowFullyDrawed, CurrentRayHitPosition);

        OnDrawBowEnd?.Invoke();
    }

    void OnAimProgress(float progress) => bow.OnAimProgress(progress);
    public void TriggerBowShoot(float extraProgress) => OnBowShoot?.Invoke(extraProgress);

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
            bow.PreparedArrow.gameObject.SetActive(false);

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
