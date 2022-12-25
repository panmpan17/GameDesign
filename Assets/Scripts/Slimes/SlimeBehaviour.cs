using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Cinemachine;
using DigitalRuby.Tween;
using XnodeBehaviourTree;
using MPack;


public class SlimeBehaviour : MonoBehaviour, ISlimeBehaviour
{
    private ITriggerFire[] triggerFires;

    [SerializeField]
    private LootTable lootTable;
    [field: SerializeField] public bool ArrowBounceOff { get; protected set; }

    [Header("Reference")]
    [SerializeField]
    private ITriggerFireGroup[] triggerFireGroups;

    [SerializeField]
    private TransformPointer player;
    public Transform PlayerTarget => player.Target;
    [SerializeField]
    private Transform fixedTarget;
    public Transform FixedTarget => fixedTarget;
    [SerializeField]
    private Transform eyePosition;
    public Transform EyePosition => eyePosition;

    [SerializeField]
    private EventReference healthChangedEvent;

    [SerializeField]
    private LayerMaskReference groundLayers;


    [Header("Dead Animation")]
    [SerializeField]
    private float coreDamagedImpulseForce;
    [SerializeField]
    private bool coreDamagePauseFrame;
    [SerializeField]
    private float sinkTime;
    [SerializeField]
    private float sinkHeight;
    public UnityEvent OnDeath;

    public float SinkHeight => sinkHeight;

    [Header("Special Effect")]
    [SerializeField]
    private CinemachineImpulseSource impulseSource;

    [Header("Sound Effect")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClipSet slamSound;
    [SerializeField]
    [FormerlySerializedAs("hitSound")]
    private AudioClipSet hitCoreSound;
    [SerializeField]
    private AudioClipSet hitBodySound;

    private SlimeCore[] _cores;
    private BehaviourTreeRunner _behaviourTreeRunner;
    private SlimeAnimationController _animationController;

    public event System.Action<Collider> OnTriggerEnterEvent;
    public event System.Action<Collision> OnCollisionEnterEvent;
    public event System.Action<Collision> OnCollisionExitEvent;

    public event System.Action<SlimeCore> OnCoreDamagedEvent;
    public event System.Action OnBounceOffEvent;

    private Vector3Tween _emergeTween;


    void Awake()
    {
        triggerFires = GetComponentsInChildren<ITriggerFire>();
        _behaviourTreeRunner = GetComponent<BehaviourTreeRunner>();
        _animationController = GetComponent<SlimeAnimationController>();
        impulseSource ??= GetComponent<CinemachineImpulseSource>();

        for (int i = 0; i < triggerFireGroups.Length; i++)
        {
            ITriggerFireGroup group = triggerFireGroups[i];
            if (!group.GroupGameObjects)
                continue;

            ITriggerFire[] triggers = group.GroupGameObjects.GetComponentsInChildren<ITriggerFire>();

            for (int e = 0; e < triggers.Length; e++)
            {
                group.TriggerAction += triggers[e].TriggerFire;
                group.ParameterTriggerAction += triggers[e].TriggerFireWithParameter;
            }
        }

        _cores = GetComponentsInChildren<SlimeCore>();
        for (int i = 0; i < _cores.Length; i++)
        {
            _cores[i].OnDamageEvent += OnCoreDamage;
        }
    }

    public void EnableTreeRunner() => _behaviourTreeRunner.enabled = true;
    public void DisableTreeRunner() => _behaviourTreeRunner.enabled = false;

#region Behaviour Tree Action
    public void TriggerFire()
    {
        for (int i = 0; i < triggerFires.Length; i++)
        {
            triggerFires[i].TriggerFire();
        }
    }
    public void TriggerFire(int parameter)
    {
        for (int i = 0; i < triggerFires.Length; i++)
        {
            triggerFires[i].TriggerFireWithParameter(parameter);
        }
    }

    public void TriggerFireGroup(int groupIndex)
    {
        triggerFireGroups[groupIndex].TriggerAction?.Invoke();
    }
    public void TriggerFireGroup(int groupIndex, int parameter)
    {
        triggerFireGroups[groupIndex].ParameterTriggerAction?.Invoke(parameter);
    }

    public void TriggerImpluse(float forceSize)
    {
        impulseSource.GenerateImpulse(forceSize);
        audioSource.PlayOneShot(slamSound);
    }
#endregion


#region Damage, Death, Loot table
    void OnCoreDamage(SlimeCore damagedCore)
    {
        if (impulseSource && coreDamagedImpulseForce > 0)
            impulseSource.GenerateImpulse(coreDamagedImpulseForce);

        if (coreDamagePauseFrame)
            StartCoroutine(C_CoreDamagedPause());

        _animationController?.SwitchToAnimation("Hurt");
        audioSource.Play(hitCoreSound);

        int aliveCount = UpdateHealth();
        if (aliveCount <= 0)
        {
            HandleDeath();
            return;
        }

        OnCoreDamagedEvent?.Invoke(damagedCore);
    }

    IEnumerator C_CoreDamagedPause()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.08f);
        Time.timeScale = 1;
    }

    void HandleDeath()
    {
        if (lootTable)
            SpawnLootTable();

        _behaviourTreeRunner.enabled = false;

        StartCoroutine(DelayDeathAnimation());
    }

    IEnumerator DelayDeathAnimation()
    {
        if (_emergeTween != null)
        {
            _emergeTween.Stop(TweenStopBehavior.DoNotModify);
            _emergeTween = null;
        }
        OnDeath.Invoke();

        yield return new WaitForSeconds(0.6f);

        _animationController?.SwitchToAnimation("Die");

        gameObject.Tween(
            gameObject,
            transform.position,
            transform.position + Vector3.down * sinkHeight,
            sinkTime,
            TweenScaleFunctions.Linear,
            (action) => transform.position = action.CurrentValue,
            (action) => Destroy(gameObject));
    }

    void SpawnLootTable()
    {
        for (int i = 0; i < lootTable.LootRules.Length; i++)
        {
            LootTable.LootRule lootRule = lootTable.LootRules[i];

            if (Random.value < lootRule.Chance)
            {
                DroppedItem item = DroppedItem.Pool.Get();
                item.transform.position = transform.position;
                item.Setup(lootRule.Type);
            }
        }
    }

    void OnDestroy()
    {
        var arrows = GetComponentsInChildren<Arrow>();
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].transform.SetParent(null);
            arrows[i].gameObject.SetActive(false);
        }
    }
#endregion

    /// <summary>
    /// Update health
    /// </summary>
    /// <returns>How many core left</returns>
    public int UpdateHealth()
    {
        int aliveCount = 0;
        for (int i = 0; i < _cores.Length; i++)
        {
            if (_cores[i].gameObject.activeSelf)
                aliveCount++;
        }
        healthChangedEvent?.Invoke((float)aliveCount / (float)_cores.Length);
        return aliveCount;
    }

    public void AlignWithGround()
    {
        Vector3 higherPosition = transform.position;
        higherPosition.y += 0.1f;
        if (Physics.Raycast(higherPosition, Vector3.down, out RaycastHit hit, 3f, groundLayers.Value))
        {
            if (Physics.Raycast(higherPosition + transform.forward, Vector3.down, out RaycastHit hit2, 3f, groundLayers.Value))
            {
                transform.LookAt(hit2.point, hit.normal);
            }
        }
    }

    public void EmergeFromTheGround(Vector3 position)
    {
        _emergeTween = gameObject.Tween(
            gameObject,
            position - new Vector3(0, SinkHeight, 0),
            position, sinkTime, TweenScaleFunctions.Linear,
            (tweenAction) =>
            {
                transform.position = tweenAction.CurrentValue;
            },
            (tweenAction) =>
            {
                AlignWithGround();
                GetComponent<Rigidbody>().isKinematic = false;
                EnableTreeRunner();
                _emergeTween = null;
            });
    }

#region Arrow shoot event
    public void OnBounceOff()
    {
        audioSource.Play(hitBodySound);
        OnBounceOffEvent?.Invoke();
    }

    public void OnArrowHitBody(Arrow arrow)
    {
        Transform arrowTransform = arrow.transform;
        Vector3 localPosition = arrowTransform.localPosition;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Update();

        arrow.TrailEmmiting = true;
        audioSource.Play(hitBodySound);

        gameObject.Tween(
            arrowTransform,
            0, 1, 0.3f, TweenScaleFunctions.Linear,
            (data) =>
            {
                if (stopwatch.DeltaTime > 0.08f)
                {
                    Vector2 value = Random.insideUnitCircle * Random.Range(0.1f, 0.13f);
                    arrowTransform.localPosition = localPosition + arrowTransform.TransformDirection(value);
                    stopwatch.Update();
                }
            },
            (data) =>
            {
                arrowTransform.localPosition = localPosition;
                arrow.TrailEmmiting = false;
            }
        );
    }
#endregion


#region Physic event
    void OnColliderEnter(Collider collider) => OnTriggerEnterEvent?.Invoke(collider);
    void OnCollisionEnter(Collision collision) => OnCollisionEnterEvent?.Invoke(collision);
    void OnCollisionExit(Collision collision) => OnCollisionExitEvent?.Invoke(collision);
#endregion
}
