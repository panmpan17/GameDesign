using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Decorate/Contact Damage")]
    public class ContactDamageNode : AbstractDecorateNode
    {
        [SerializeField]
        private float damangeAmount;

        private bool _damaged;
        private PlayerBehaviour _playerBehaviour;


        protected override void OnStart()
        {
            _damaged = false;
            context.slimeBehaviour.OnCollisionEnterEvent += OnCollisionEnter;

            if (_playerBehaviour == null)
                context.slimeBehaviour.OnCollisionExitEvent += OnCollisionExit;
            else
            {
                _playerBehaviour.OnDamage(damangeAmount);
            }
        }

        protected override void OnStop()
        {
            context.slimeBehaviour.OnCollisionEnterEvent -= OnCollisionEnter;
        }

        protected override State OnUpdate()
        {
            return _child.Update();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!_damaged && collision.gameObject.CompareTag(PlayerBehaviour.Tag))
            {
                _playerBehaviour = collision.gameObject.GetComponent<PlayerBehaviour>();
                _playerBehaviour.OnDamage(damangeAmount);
                _damaged = true;
            }
        }
        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag(PlayerBehaviour.Tag))
            {
                _playerBehaviour = null;
                context.slimeBehaviour.OnCollisionExitEvent -= OnCollisionExit;
            }
        }
    }
}