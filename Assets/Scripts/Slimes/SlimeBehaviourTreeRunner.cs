using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using MPack;


public class SlimeBehaviourTreeRunner : BehaviourTreeRunner
{
    private ITriggerFire[] triggerFires;

    [SerializeField]
    private Transform player;
    public Transform PlayerTarget => player;
    [SerializeField]
    private Transform fixedTarget;
    public Transform FixedTarget => fixedTarget;

    [SerializeField]
    private LootTable lootTable;

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


        _cores = GetComponentsInChildren<SlimeCore>();
        for (int i = 0; i < _cores.Length; i++)
        {
            _cores[i].OnDamageEvent += OnCoreDamage;
        }

        sinkTimer.Timer.Running = false;
    }

#if UNITY_EDITOR
    protected override void Start()
    {
        
        if (tree == null)
        {
            Debug.LogError("This slime doesn't have behaviour tree", this);
            enabled = false;
        }

        context = CreateBehaviourTreeContext();
        tree = tree.Clone();
        tree.Bind(context);
    }
#endif

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


#region Damage, Death, Loot table
    void OnCoreDamage()
    {
        for (int i = 0; i < _cores.Length; i++)
        {
            if (_cores[i].gameObject.activeSelf)
            {
                return;
            }
        }

        var arrows = GetComponentsInChildren<Arrow>();
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].transform.SetParent(null);
            arrows[i].gameObject.SetActive(false);
        }

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
#endregion


    protected override Context CreateBehaviourTreeContext()
    {
        return Context.Create(this);
    }

    void OnColliderEnter(Collider collider) => OnTriggerEnterEvent?.Invoke(collider);
    void OnCollisionEnter(Collision collision) => OnCollisionEnterEvent?.Invoke(collision);
    void OnCollisionExit(Collision collision) => OnCollisionExitEvent?.Invoke(collision);
}
