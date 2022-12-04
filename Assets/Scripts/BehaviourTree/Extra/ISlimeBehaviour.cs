using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlimeBehaviour
{
    Transform EyePosition { get; }
    Transform FixedTarget { get; }
    Transform PlayerTarget { get; }

    event System.Action<Collider> OnTriggerEnterEvent;
    event System.Action<Collision> OnCollisionEnterEvent;
    event System.Action<Collision> OnCollisionExitEvent;

    void TriggerFire();
    void TriggerFire(int parameter);
    void TriggerFireGroup(int groupIndex);
    void TriggerFireGroup(int groupIndex, int parameter);
    void TriggerImpluse(float forceSize);
}
