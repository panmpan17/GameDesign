using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Decorate/Contact Damage")]
    public class ContactDamageNode : AbstractDecorateNode
    {
        [SerializeField]
        private float damangeAmount;

        [Output(connectionType: ConnectionType.Override)]
        public BehaviourPort OnContactOutput;

        private bool _damaged;
        private ICanBeDamage _playerBehaviour;

        private AbstractBehaviourNode _onContactChild;
        private State _onContactState = State.Running;

        public override void OnInitial()
        {
            base.OnInitial();

            NodePort port = GetOutputPort("OnContactOutput");
            if (port.IsConnected)
                _onContactChild = (AbstractBehaviourNode)port.Connection.node;
        }


        protected override void OnStart()
        {
            _damaged = false;
            _onContactState = State.Running;
            context.slimeBehaviour.OnCollisionEnterEvent += OnCollisionEnter;

            if (_playerBehaviour == null)
                context.slimeBehaviour.OnCollisionExitEvent += OnCollisionExit;
            else
            {
                _damaged = true;
                _playerBehaviour.OnDamage(damangeAmount);
            }
        }

        protected override void OnStop()
        {
            context.slimeBehaviour.OnCollisionEnterEvent -= OnCollisionEnter;
        }

        protected override State OnUpdate()
        {
            if (_damaged && _onContactState == State.Running)
            {
                _onContactState = _onContactChild.Update(); 
            }

            return _child.Update();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!_damaged && collision.gameObject.CompareTag("Player"))
            {
                _playerBehaviour = collision.gameObject.GetComponent<ICanBeDamage>();
                _playerBehaviour.OnDamage(damangeAmount);
                _damaged = true;
            }
        }
        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _playerBehaviour = null;
                context.slimeBehaviour.OnCollisionExitEvent -= OnCollisionExit;
            }
        }
    }
}