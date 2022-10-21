using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using MPack;


public class SlimeBehaviourTreeRunner : BehaviourTreeRunner
{
    private ITriggerFire[] triggerFires;

    [SerializeField]
    private ITriggerFireGroup[] triggerFireGroups;

    [SerializeField]
    private TransformPointer player;
    public Transform PlayerTarget => player.Target;
    [SerializeField]
    private Transform fixedTarget;
    public Transform FixedTarget => fixedTarget;

    [SerializeField]
    private LootTable lootTable;

    [SerializeField]
    private EventReference healthChangedEvent;

    [Header("Dead Animation")]
    [SerializeField]
    private Vector3LerpTimer sinkTimer;
    [SerializeField]
    private float sinkHeight;

    public event System.Action<Collider> OnTriggerEnterEvent;
    public event System.Action<Collision> OnCollisionEnterEvent;
    public event System.Action<Collision> OnCollisionExitEvent;

    private SlimeCore[] _cores;


    void Awake()
    {
        triggerFires = GetComponentsInChildren<ITriggerFire>();

        for (int i = 0; i < triggerFireGroups.Length; i++)
        {
            ITriggerFireGroup group = triggerFireGroups[i];
            ITriggerFire[] triggers = group.GroupGameObjects.GetComponentsInChildren<ITriggerFire>();

            for (int e = 0; e < triggers.Length; e++)
                group.TriggerAction += triggers[e].TriggerFire;
        }

        _cores = GetComponentsInChildren<SlimeCore>();
        for (int i = 0; i < _cores.Length; i++)
        {
            _cores[i].OnDamageEvent += OnCoreDamage;
        }

        sinkTimer.Timer.Running = false;
    }

    protected override void Start()
    {
#if UNITY_EDITOR
        if (tree == null)
        {
            Debug.LogError("This slime doesn't have behaviour tree", this);
            enabled = false;
        }
#endif

        context = CreateBehaviourTreeContext();
        tree = tree.Clone();
        tree.Bind(context);
    }

    protected override void Update()
    {
        if (!sinkTimer.Timer.Running)
        {
            tree.Update();
            return;
        }

        if (sinkTimer.Timer.UpdateEnd)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = sinkTimer.Value;
    }

    public void TriggerFire()
    {
        for (int i = 0; i < triggerFires.Length; i++)
        {
            triggerFires[i].TriggerFire();
        }
    }

    public void TriggerFireGroup(int groupIndex)
    {
        triggerFireGroups[groupIndex].TriggerAction.Invoke();
    }


#region Damage, Death, Loot table
    void OnCoreDamage()
    {
        int aliveCount = 0;
        for (int i = 0; i < _cores.Length; i++)
        {
            if (_cores[i].gameObject.activeSelf)
                aliveCount++;
        }

        // float healthPercent
        healthChangedEvent?.Invoke((float)aliveCount / (float)_cores.Length);

        if (aliveCount <= 0)
            HandleDeath();
    }

    void HandleDeath()
    {
        if (lootTable)
            SpawnLootTable();

        sinkTimer.From = transform.position;
        sinkTimer.To = transform.position + Vector3.down * sinkHeight;
        sinkTimer.Timer.Reset();
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


    protected override Context CreateBehaviourTreeContext()
    {
        return Context.Create(this);
    }

    void OnColliderEnter(Collider collider) => OnTriggerEnterEvent?.Invoke(collider);
    void OnCollisionEnter(Collision collision) => OnCollisionEnterEvent?.Invoke(collision);
    void OnCollisionExit(Collision collision) => OnCollisionExitEvent?.Invoke(collision);


    [System.Serializable]
    public class ITriggerFireGroup
    {
        public System.Action TriggerAction;
        public GameObject GroupGameObjects;
    }
}
