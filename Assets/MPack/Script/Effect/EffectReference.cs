using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    [CreateAssetMenu(menuName = "MPack/Effect Reference", order = 0)]
    public class EffectReference : ScriptableObject
    {
        public ParticleSystem Prefab;

        public Stack<ParticleSystem> Pools;
        public Stack<EffectQueue> WaitingList;

        void OnEnable()
        {
            Pools = new Stack<ParticleSystem>();
            WaitingList = new Stack<EffectQueue>();
        }

        public ParticleSystem GetFreshEffect()
        {
            while (Pools.Count > 0)
            {
                ParticleSystem particle = Pools.Pop();
                if (particle != null)
                    return particle;
            }

            return GameObject.Instantiate(Prefab);
        }

        public void Put(ParticleSystem effect)
        {
            effect.Stop();
            Pools.Push(effect);
        }

        public void AddWaitingList(EffectQueue queue)
        {
            WaitingList.Push(queue);
        }

        public void AddWaitingList(Vector3 position, Quaternion rotation, bool useScaleTime=true)
        {
            WaitingList.Push(new EffectQueue {
                Position = position,
                Rotation = rotation,
                UseScaleTime = useScaleTime,
            });
        }

        public void Clear()
        {
            Pools.Clear();
            WaitingList.Clear();
        }

        public struct EffectQueue
        {
            public Transform Parent;
            public Vector3 Position;
            public Quaternion Rotation;
            public bool UseScaleTime;
        }
    }
}