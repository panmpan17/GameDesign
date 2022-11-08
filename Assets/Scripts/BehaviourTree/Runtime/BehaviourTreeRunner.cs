// using System;
using UnityEngine;
using UnityEngine.Events;
using MPack;
using Cinemachine;


namespace XnodeBehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour, ISlimeBehaviour
    {
        [SerializeField]
        private BehaviourTreeGraph graph;

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
        private Transform eyePosition;
        public Transform EyePosition => eyePosition;

        [SerializeField]
        private LootTable lootTable;

        [SerializeField]
        private EventReference healthChangedEvent;

        [Header("Dead Animation")]
        [SerializeField]
        private Vector3LerpTimer sinkTimer;
        [SerializeField]
        private float sinkHeight;
        public UnityEvent OnDeath;

        [Header("Special Effect")]
        [SerializeField]
        private CinemachineImpulseSource impulseSource;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClipSet slamSound;

        private SlimeCore[] _cores;

        public event System.Action<Collider> OnTriggerEnterEvent;
        public event System.Action<Collision> OnCollisionEnterEvent;
        public event System.Action<Collision> OnCollisionExitEvent;


        void Awake()
        {
            triggerFires = GetComponentsInChildren<ITriggerFire>();

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

            sinkTimer.Timer.Running = false;
        }

        void Start()
        {
            TheKiwiCoder.Context context = TheKiwiCoder.Context.Create(gameObject, this);
            graph = (BehaviourTreeGraph) graph.Copy();
            graph.OnInitial(context);
        }

        void Update()
        {
            graph.Update();
        }


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

#region Damage, Death, Loot table
        void OnCoreDamage()
        {
            int aliveCount = UpdateHealth();
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

            OnDeath?.Invoke();
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

        void OnColliderEnter(Collider collider) => OnTriggerEnterEvent?.Invoke(collider);
        void OnCollisionEnter(Collision collision) => OnCollisionEnterEvent?.Invoke(collision);
        void OnCollisionExit(Collision collision) => OnCollisionExitEvent?.Invoke(collision);
    }
}