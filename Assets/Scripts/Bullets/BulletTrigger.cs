using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class BulletTrigger : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed;

    public void Trigger()
    {
        BulletBillboards.ins.FireBullet(transform.position, transform.forward * bulletSpeed);
    }
}
