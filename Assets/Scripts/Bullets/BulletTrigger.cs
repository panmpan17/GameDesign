using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public interface ITriggerFire
{
    void TriggerFire();
}

public class BulletTrigger : MonoBehaviour, ITriggerFire
{
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private BulletType bulletType;

    public void Trigger()
    {
        var bullet = bulletType.Pool.Get();
        bullet.transform.SetPositionAndRotation(transform.position, BulletBillboards.ins.FaceCameraRotation);
        bullet.Shoot(transform.forward * bulletSpeed);
        // BulletBillboards.ins.FireBullet(transform.position, transform.forward * bulletSpeed);
    }

    public void TriggerFire()
    {
        var bullet = bulletType.Pool.Get();
        bullet.transform.SetPositionAndRotation(transform.position, BulletBillboards.ins.FaceCameraRotation);
        bullet.Shoot(transform.forward * bulletSpeed);
        // BulletBillboards.ins.FireBullet(transform.position, transform.forward * bulletSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
