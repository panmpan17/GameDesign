using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class PhysicCanon : MonoBehaviour, ITriggerFire
{
    [SerializeField]
    private PhysicSimulate physicSimulate;
    [SerializeField]
    private float startForce;
    [SerializeField]
    private BulletType bulletType;

    [Header("Gizmos Simulate")]
    [SerializeField]
    private float deltaTime = 0.2f;
    [SerializeField]
    private float simulateCount = 50;

    public void TriggerFire()
    {
        physicSimulate.SetPositionAndVelocity(transform.position, startForce * transform.forward);
        var canonShell = bulletType.Pool.Get();
        canonShell.Shoot(physicSimulate);
    }

    void OnDrawGizmosSelected()
    {
        physicSimulate.SetPositionAndVelocity(transform.position, startForce * transform.forward);

        Gizmos.DrawSphere(physicSimulate.Position, 0.1f);
        for (int i = 0; i < simulateCount; i++)
        {
            physicSimulate.Update(deltaTime);
            Gizmos.DrawSphere(physicSimulate.Position, 0.1f);
        }
    }
}
