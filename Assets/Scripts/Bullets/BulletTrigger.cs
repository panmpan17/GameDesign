using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public interface ITriggerFire
{
    void TriggerFire();
    void TriggerFireWithParameter(int parameter);
}

public class BulletTrigger : MonoBehaviour, ITriggerFire
{
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private BulletType bulletType;
    [SerializeField]
    private AudioClipSet shootSound;
    [SerializeField]
    private AudioSource audioSource;

    public void TriggerFire()
    {
        var bullet = bulletType.Pool.Get();
        if (bulletType.UseBillboardRotate)
            bullet.transform.SetPositionAndRotation(transform.position, BulletBillboards.ins.FaceCameraRotation);
        else
            bullet.transform.position = transform.position;
        bullet.Shoot(transform.forward * bulletSpeed);

        audioSource?.Play(shootSound);
        // BulletBillboards.ins.FireBullet(transform.position, transform.forward * bulletSpeed);
    }

    public void TriggerFireWithParameter(int parameter)
    {}

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
