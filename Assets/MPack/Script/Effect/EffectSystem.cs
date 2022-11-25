using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack
{
    public class EffectSystem : MonoBehaviour
    {
        [SerializeField]
        private EffectReference[] listenEffects;

        void LateUpdate()
        {
            for (int i = 0; i < listenEffects.Length; i++)
            {
                EffectReference effectReference = listenEffects[i];

                while (effectReference.WaitingList.Count > 0)
                {
                    EffectReference.EffectQueue effectQueue = effectReference.WaitingList.Pop();

                    ParticleSystem newEffect = effectReference.GetFreshEffect();

                    Transform effectTransform = newEffect.transform;
                    effectTransform.SetParent(effectQueue.Parent);
                    effectTransform.SetPositionAndRotation(effectQueue.Position, effectQueue.Rotation);

                    ParticleSystem.MainModule main = newEffect.main;
                    main.useUnscaledTime = !effectQueue.UseScaleTime;
                    newEffect.Play();

                    StartCoroutine(WaitToCollectEffect(effectReference, newEffect, effectQueue.UseScaleTime, newEffect.main.duration));
                }
            }
        }

        IEnumerator WaitToCollectEffect(EffectReference effectReference, ParticleSystem effect, bool useScaleTime, float duration)
        {
            if (useScaleTime)
                yield return new WaitForSeconds(duration);
            else
                yield return new WaitForSecondsRealtime(duration);

            effectReference.Put(effect);
        }

        void OnDestroy()
        {
            for (int i = 0; i < listenEffects.Length; i++)
            {
                listenEffects[i].Clear();
            }
        }
    }
}