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

    [SerializeField]
    private Bow bow;
    public event System.Action<BowParameter, BowParameter> OnBowParameterChanged;

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
        input.OnInteract += OnInteract;
        input.OnEatApple += OnEatApple;
        input.OnMinimapEnlargeDown += OnEnlargeMinimap;
        input.OnMinimapEnlargeUp += OnShrinkMinimap;
        // inpu

        movement.OnRollEvent += OnRoll;
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
        _currentRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(_currentRay, out RaycastHit shootTargetHit, 50, hitLayers))
            CurrentRayHitPosition = shootTargetHit.point;
        else
            CurrentRayHitPosition = _currentRay.GetPoint(50);

        UpdateInteractCheck();
    }

    private void UpdateInteractCheck()
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

        if (_interactObject.GetComponent<TreasureChest>() is var chest && chest)
        {
            chest.Open();
            canInteractEvent?.Invoke(false);
            movement.FaceRotationWithoutRotateFollowTarget(chest.transform.position);
        }

        if (_interactObject.GetComponent<PortalGate>() is var portal && portal)
        {
            InstantTeleportTo(portal.Teleport());
            canInteractEvent?.Invoke(false);
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
        }
    }

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
    }

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


    public void PickItemUp(ItemType itemType)
    {
        inventory.ChangeCoreCount(1);
    }

    public void UpgradeBow(BowParameter upgradeParameter)
    {
        bow.UpgradeBow(upgradeParameter);
        OnBowParameterChanged?.Invoke(bow.CurrentParameter, upgradeParameter);
    }



    void OnDestroy()
    {
        focusEvent.InvokeEvents -= FocusCursor;
        dialogueEndEvent.InvokeEvents -= DialogueUIEnd;
        AbstractMenu.OnFirstMenuOpen -= OnFirstMenuOpen;
        AbstractMenu.OnLastMenuClose -= OnLastMenuClose;
    }
}
