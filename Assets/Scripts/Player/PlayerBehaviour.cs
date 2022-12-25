using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MPack;


public class PlayerBehaviour : MonoBehaviour, ICanBeDamage
{
    public const string Tag = "Player";
    private const string RightClickTag = "RightClickInteractive";

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

    [Header("Bow")]
    [SerializeField]
    private Bow bow;
    public event System.Action<BowParameter, BowParameter> OnBowParameterChanged;
    public event System.Action OnBowUpgrade;
    [SerializeField]
    private float waitShootTime;
    private bool _waitShoot;
    private Stopwatch _waitShootStopwatch;
    [SerializeField]
    private float keepTargetTime;

    private Ray _currentRay;
    private Stopwatch _focusedStopwatch;
    private Transform _focusedTarget;
    private Vector3 _bowShootPoint;
    public Vector3 CurrentRayHitPosition { get; private set; }


    [Header("Reference")]
    [SerializeField]
    private TransformPointer bodyPointer;
    [SerializeField]
    private TransformPointer feetPointer;
    [SerializeField]
    private Transform body;
    [SerializeField]
    private Transform feet;

    [Header("Health")]
    [SerializeField]
    private float maxHealth;
    private float _health;
    [SerializeField]
    private EventReference healthChangeEvent;
    [SerializeField]
    private float appleHealPoint;

    [Header("Others")]
    [SerializeField]
    private LayerMask hitLayers;
    [SerializeField]
    private EventReference pauseEvent;
    [SerializeField]
    private EventReference focusEvent;
    [SerializeField]
    private EventReference enlargeMinimapEvent;

    [Header("Interact")]
    [SerializeField]
    private LayerMask interactLayer;
    [SerializeField]
    private float interactRaycastDistance;
    [SerializeField]
    private EventReference canInteractEvent;
    [SerializeField]
    private EventReference dialogueEndEvent;

    private GameObject _interactObject;
    private bool LastCanInteract => _interactObject != null;
    private Collider[] _interactColliders = new Collider[1];
    
    [Header("Inventory")]
    [SerializeField]
    private Inventory inventory;

#if UNITY_EDITOR
    [SerializeField]
    private ValueWithEnable<int> startCoreCount;
    [SerializeField]
    private ValueWithEnable<int> startAppleCount;
#endif

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool focusCursorWhenPointerDown;
#endif


#region Actions
    public event System.Action OnDrawBow;
    public event System.Action OnDrawBowEnd;
    public event System.Action<float> OnBowShoot;
    public event System.Action OnPickItem;

    public event System.Action OnHurt;
    public event System.Action OnDeath;
    public event System.Action OnRevive;
    public event System.Action OnHeal;
    public event System.Action OnDodgeSuccess;
#endregion

    public bool CursorFocued { get; protected set; }
    public bool IsDrawingBow { get; private set; }
    public bool IsDead => _handleDeath;

    private int _walkingCameraIndex;

    private bool _handleDeath = false;
    private bool _invincible = false;


    void Awake()
    {
        input = GetComponent<InputInterface>();
        input.OnAimDown += OnAimDown;
        input.OnAimUp += OnAimUp;
        input.OnEscap += OnEscap;
        input.OnInteract += OnInteract;
        input.OnEatApple += OnEatApple;
        input.OnMinimapEnlargeDown += OnEnlargeMinimap;
        input.OnMinimapEnlargeUp += OnShrinkMinimap;
        // inpu

        movement.OnRollEvent += OnRoll;
        movement.OnRollEndEvent += OnRollEnd;
        // movement.OnJumpEndEvent += OnJumpEnd;

        animation.OnAimAnimatinoChanged += OnAimProgress;

        focusEvent.InvokeEvents += FocusCursor;
        dialogueEndEvent.InvokeEvents += DialogueUIEnd;
        AbstractMenu.OnFirstMenuOpen += OnFirstMenuOpen;
        AbstractMenu.OnLastMenuClose += OnLastMenuClose;

        _walkingCameraIndex = CameraSwitcher.GetCameraIndex("Walk");

        mainCamera = mainCamera == null ? Camera.main : mainCamera;
        bodyPointer.Target = body;
        feetPointer.Target = feet;

        bow.Setup(this);

#if UNITY_EDITOR
        if (startCoreCount.Enable) inventory.ChangeCoreCount(startCoreCount.Value);
        if (startAppleCount.Enable) inventory.ChangeAppleCount(startAppleCount.Value);
#endif
    }

    void Start()
    {
        _health = maxHealth;
        healthChangeEvent?.Invoke(1);

        OnBowParameterChanged?.Invoke(bow.CurrentParameter, bow.CurrentParameter);

        FocusCursor();
    }

    void Update()
    {
        UpdateAimingRay();
        UpdateInteractCheck();
    }

    void UpdateAimingRay()
    {
        _currentRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(_currentRay, out RaycastHit shootTargetHit, 50, hitLayers))
        {
            CurrentRayHitPosition = _bowShootPoint = shootTargetHit.point;

            if (shootTargetHit.collider.CompareTag(SlimeCore.Tag))
            {
                _focusedStopwatch.Update();
                _focusedTarget = shootTargetHit.collider.transform;
                _bowShootPoint = _focusedTarget.position;
            }
            else
            {
                if (_focusedTarget && _focusedStopwatch.DeltaTime <= keepTargetTime)
                    _bowShootPoint = _focusedTarget.position;
            }
        }
        else
        {
            CurrentRayHitPosition = _bowShootPoint = _currentRay.GetPoint(50);

            if (_focusedTarget && _focusedStopwatch.DeltaTime <= keepTargetTime)
                _bowShootPoint = _focusedTarget.position;
        }
    }

    void UpdateInteractCheck()
    {
        bool isOverlap = Physics.OverlapSphereNonAlloc(body.position, interactRaycastDistance, _interactColliders, interactLayer) > 0;

        if (isOverlap)
        {
            if (!LastCanInteract)
                CheckGameObjectIsRightClickInteractable(_interactColliders[0].gameObject);
        }
        else
        {
            if (LastCanInteract)
            {
                _interactObject = null;
                canInteractEvent.Invoke(false);
            }
        }
    }

    void CheckGameObjectIsRightClickInteractable(GameObject interactGameObject)
    {
        if (!interactGameObject.CompareTag("RightClickInteractive"))
            return;

        _interactObject = interactGameObject;
        canInteractEvent.Invoke(true);
    }


#region Input Events
    void OnAimDown()
    {
        if (!CursorFocued)
        {
#if UNITY_EDITOR
            if (focusCursorWhenPointerDown)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    FocusCursor();
            }
#endif
            return;
        }

        if (movement.IsRolling)
        {
            _waitShoot = true;
            _waitShootStopwatch.Update();
            return;
        }

        IsDrawingBow = true;

        _focusedTarget = null;

        bow.OnAimDown();
        OnDrawBow?.Invoke();
    }

    void OnAimUp()
    {
        _waitShoot = false;
        if (!CursorFocued || !IsDrawingBow)
            return;

        IsDrawingBow = false;

        _focusedTarget = null;

        bool isBowFullyDrawed = animation.IsDrawArrowFullyPlayed;
        bow.OnAimUp(isBowFullyDrawed, _bowShootPoint);

        OnDrawBowEnd?.Invoke();
    }

    void OnAimProgress(float progress) => bow.OnAimProgress(progress);
    public void TriggerBowShoot(float extraProgress) => OnBowShoot?.Invoke(extraProgress);

    void OnEscap()
    {
        pauseEvent.Invoke();
#if UNITY_EDITOR
        if (focusCursorWhenPointerDown)
        {
            OutFocusCursor();
        }
#endif
    }

    public void OnInteract()
    {
        if (!CursorFocued)
            return;
        if (!_interactObject)
            return;

        if (_interactObject.GetComponent<NPCControl>() is var npc && npc)
        {
            npc.StartDialogue();

            canInteractEvent?.Invoke(false);
            movement.FaceRotationWithoutRotateFollowTarget(npc.transform.position);
            return;
        }

        else if (_interactObject.GetComponent<TreasureChest>() is var chest && chest)
        {
            chest.Open();
            canInteractEvent?.Invoke(false);
            movement.FaceRotationWithoutRotateFollowTarget(chest.transform.position);
        }

        else if (_interactObject.GetComponent<PortalGate>() is var portal && portal)
        {
            InstantTeleportTo(portal.Teleport());
            canInteractEvent?.Invoke(false);
        }

        else if (_interactObject.GetComponent<PickupFlower>() is var flower && flower)
        {
            flower.Pickup();
            inventory.FlowerEvent.Invoke(true);
        }
    }

    void OnEatApple()
    {
        if (!CursorFocued)
            return;

        if (_health >= maxHealth)
            return;

        if (inventory.AppleCount >= 1)
        {
            inventory.ChangeAppleCount(-1);
            _health += appleHealPoint;
            healthChangeEvent?.Invoke(Mathf.Clamp(_health / maxHealth, 0, 1));
            OnHeal?.Invoke();
        }
    }

    void OnEnlargeMinimap()
    {
        if (!CursorFocued)
            return;

        enlargeMinimapEvent.Invoke(true);
    }
    void OnShrinkMinimap()
    {
        if (!CursorFocued)
            return;

        enlargeMinimapEvent.Invoke(false);
    }
#endregion


#region Movement Event
    void OnRoll()
    {
        if (!CursorFocued)
            return;

        if (IsDrawingBow)
        {
            IsDrawingBow = false;
            bow.PreparedArrow.gameObject.SetActive(false);

            CameraSwitcher.ins.SwitchTo(_walkingCameraIndex);
            OnDrawBowEnd?.Invoke();
            bow.OnAimProgress(0);
        }
    }

    void OnRollEnd()
    {
        if (_waitShoot && _waitShootStopwatch.DeltaTime < waitShootTime)
        {
            _waitShoot = false;
            IsDrawingBow = true;

            bow.OnAimDown();
            OnDrawBow?.Invoke();
        }
    }

    // void OnJumpEnd()
    // {}
#endregion


#region Health relative
    public bool TryToDamage()
    {
        if (movement.IsRolling)
        {
            OnDodgeSuccess?.Invoke();
            return false;
        }

        return !_handleDeath && !_invincible;
    }

    public void OnDamage(float amount)
    {
        if (!TryToDamage()) return;

        _health -= amount;

        healthChangeEvent?.Invoke(Mathf.Clamp(_health / maxHealth, 0, 1));

        if (_health > 0)
        {
            OnHurt.Invoke();
            return;
        }

        _handleDeath = true;
        OnDeath?.Invoke();
    }

    public void InstantDeath()
    {
        if (_handleDeath)
            return;

        _health = 0;
        healthChangeEvent?.Invoke(0);

        _handleDeath = true;
        OnDeath?.Invoke();
    }

    public void ReviveAtSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        _handleDeath = false;

        _health = maxHealth;
        healthChangeEvent?.Invoke(1f);

        InstantTeleportTo(spawnPoint.transform.position);
        OnRevive?.Invoke();

        bow.DisableAllArrows();
    }

    public void SetInvincible(bool status) => _invincible = status;
#endregion


    public void InstantTeleportTo(Vector3 position)
    {
        movement.CharacterController.enabled = false;
        transform.position = position;
        movement.CharacterController.enabled = true;

        CameraSwitcher.ins.SwitchTo(_walkingCameraIndex);
        CameraSwitcher.GetCamera(_walkingCameraIndex).CancelDamping();
    }

    public void FocusCursor()
    {
        CursorFocued = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void OutFocusCursor()
    {
        CursorFocued = false;
        Cursor.lockState = CursorLockMode.None;
    }


#region UI events
    void OnFirstMenuOpen(AbstractMenu menu)
    {
        input.Disable();
        OutFocusCursor();
    }

    void OnLastMenuClose(AbstractMenu menu)
    {
        input.Enable();
        FocusCursor();
    }

    void DialogueUIEnd()
    {
        if (LastCanInteract)
        {
            canInteractEvent.Invoke(true);
        }
    }
#endregion


#region Game Item Interact
    public void PickItemUp(ItemType itemType)
    {
        inventory.ChangeCoreCount(1);
        OnPickItem?.Invoke();
    }

    public void UpgradeBow(BowParameter upgradeParameter)
    {
        bow.UpgradeBow(upgradeParameter);
        OnBowParameterChanged?.Invoke(bow.CurrentParameter, upgradeParameter);
        OnBowUpgrade?.Invoke();
    }
#endregion


    void OnDestroy()
    {
        focusEvent.InvokeEvents -= FocusCursor;
        dialogueEndEvent.InvokeEvents -= DialogueUIEnd;
        AbstractMenu.OnFirstMenuOpen -= OnFirstMenuOpen;
        AbstractMenu.OnLastMenuClose -= OnLastMenuClose;
    }
}
