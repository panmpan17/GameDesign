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
    private PlayerMovement movement;
    [SerializeField]
    private new PlayerAnimation animation;
    private InputInterface input;
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

    [SerializeField]
    private BowParameter[] bowUpgrades;
    private bool[] _upgradeUnlocks;

    private Ray _currentRay;
    private Stopwatch _focusedStopwatch;
    private Transform _focusedTarget;
    private Vector3 _bowShootPoint;
    public Vector3 CurrentRayHitPosition { get; private set; }

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
    
    [Header("Inventory")]
    [SerializeField]
    private Inventory inventory;

    [Header("Save")]
    [SerializeField]
    private SaveDataReference saveDataReference;
    [SerializeField]
    private EventReference saveDataExtractEvent;
    [SerializeField]
    private EventReference saveDataRestoreEvent;

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private ValueWithEnable<int> startCoreCount;
    [SerializeField]
    private ValueWithEnable<int> startAppleCount;
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
        input.OnEatApple += OnEatApple;
        input.OnMinimapEnlargeDown += OnEnlargeMinimap;
        input.OnMinimapEnlargeUp += OnShrinkMinimap;

        movement.OnRollEvent += OnRoll;
        movement.OnRollEndEvent += OnRollEnd;

        animation.OnAimAnimatinoChanged += OnAimProgress;

        focusEvent.InvokeEvents += FocusCursor;
        AbstractMenu.OnFirstMenuOpen += OnFirstMenuOpen;
        AbstractMenu.OnLastMenuClose += OnLastMenuClose;
        saveDataExtractEvent.InvokeEvents += OnSaveDataExtract;
        saveDataRestoreEvent.InvokeEvents += OnSaveDataRestore;

        _walkingCameraIndex = CameraSwitcher.GetCameraIndex("Walk");

        mainCamera = mainCamera == null ? Camera.main : mainCamera;

        bow.Setup(this);
        _upgradeUnlocks = new bool[bowUpgrades.Length];

#if UNITY_EDITOR
        if (startCoreCount.Enable) inventory.CoreCount = startCoreCount.Value;
        if (startAppleCount.Enable) inventory.AppleCount = startAppleCount.Value;
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

    void OnEatApple()
    {
        if (!CursorFocued)
            return;

        if (_health >= maxHealth)
            return;

        if (inventory.AppleCount >= 1)
        {
            inventory.AppleCount -= 1;
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
#endregion


#region Game Item Interact
    public void PickItemUp(ItemType itemType)
    {
        inventory.CoreCount += 1;
        OnPickItem?.Invoke();
    }

    public void UpgradeBow(BowParameter upgradeParameter)
    {
        bow.UpgradeBow(upgradeParameter);
        OnBowParameterChanged?.Invoke(bow.CurrentParameter, upgradeParameter);
        OnBowUpgrade?.Invoke();

        for (int i = 0; i < bowUpgrades.Length; i++)
        {
            if (bowUpgrades[i] == upgradeParameter)
            {
                _upgradeUnlocks[i] = true;
                return;
            }
        }
    }
    public void UpgradeBowBySave(BowParameter upgradeParameter)
    {
        bow.UpgradeBow(upgradeParameter);
        OnBowParameterChanged?.Invoke(bow.CurrentParameter, upgradeParameter);
    }
#endregion

    void OnSaveDataExtract()
    {
        saveDataReference.Data.AppleCount = inventory.AppleCount;
        saveDataReference.Data.CoreCount = inventory.CoreCount;
        saveDataReference.Data.HasFollower = inventory.HasFlower;

        saveDataReference.Data.HealthPoint = _health;
        saveDataReference.Data.BowUpgrade1Aquired = _upgradeUnlocks[0];
        saveDataReference.Data.BowUpgrade2Aquired = _upgradeUnlocks[1];
        saveDataReference.Data.BowUpgrade3Aquired = _upgradeUnlocks[2];
    }

    void OnSaveDataRestore()
    {
        inventory.AppleCount = saveDataReference.Data.AppleCount;
        inventory.CoreCount = saveDataReference.Data.CoreCount;
        inventory.HasFlower = saveDataReference.Data.HasFollower;

        _health = saveDataReference.Data.HealthPoint;
        healthChangeEvent?.Invoke(_health / maxHealth);

        _upgradeUnlocks[0] = saveDataReference.Data.BowUpgrade1Aquired;
        _upgradeUnlocks[1] = saveDataReference.Data.BowUpgrade2Aquired;
        _upgradeUnlocks[2] = saveDataReference.Data.BowUpgrade3Aquired;

        if (_upgradeUnlocks[0]) UpgradeBowBySave(bowUpgrades[0]);
        if (_upgradeUnlocks[1]) UpgradeBowBySave(bowUpgrades[1]);
        if (_upgradeUnlocks[2]) UpgradeBowBySave(bowUpgrades[2]);
    }


    void OnDestroy()
    {
        focusEvent.InvokeEvents -= FocusCursor;
        AbstractMenu.OnFirstMenuOpen -= OnFirstMenuOpen;
        AbstractMenu.OnLastMenuClose -= OnLastMenuClose;
        saveDataExtractEvent.InvokeEvents -= OnSaveDataExtract;
        saveDataRestoreEvent.InvokeEvents -= OnSaveDataRestore;
    }

#region Console window command
    [ConsoleCommand("fullhealth")]
    public static void Command_RestoreFullHealth()
    {
        PlayerBehaviour player = GameManager.ins.Player;
        player._health = player.maxHealth;
        player.healthChangeEvent.Invoke(1);
    }

    [ConsoleCommand("dead")]
    public static void Command_Dead()
    {
        PlayerBehaviour player = GameManager.ins.Player;
        player.InstantDeath();
    }

    [ConsoleCommand("upgrade :int")]
    public static void Command_UnlockUpgrade(int index)
    {
        PlayerBehaviour player = GameManager.ins.Player;
        player.UpgradeBow(player.bowUpgrades[index]);
    }
#endregion
}
