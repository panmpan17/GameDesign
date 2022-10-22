using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    public class EffectTrigger : MonoBehaviour
    {
        [SerializeField]
        private EffectReference effect;
        [SerializeField]
        private bool useScaledTime = true;
        [SerializeField]
        private bool triggerOnEnable;
        [SerializeField]
        private bool triggerOnDisable;
        [SerializeField]
        private bool triggerOnStart;
        [SerializeField]
        private bool triggerOnDestroy;

        void Start()
        {
            if (triggerOnStart)
                Trigger();
        }

        void OnEnable()
        {
            if (triggerOnEnable)
                Trigger();
        }

        void OnDisable()
        {
            if (triggerOnDisable)
                Trigger();
        }

        void OnDestroy()
        {
            if (triggerOnDestroy)
                Trigger();
        }

        public void Trigger()
        {
            effect.AddWaitingList(new EffectReference.EffectQueue
            {
                Parent = null,
                Position = transform.position,
                UseScaleTime = useScaledTime,
            });
        }
    }
}