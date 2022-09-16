using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class BulletBillboards : MonoBehaviour
{
    public static BulletBillboards ins;

    [SerializeField]
    private Transform mainCameraTransform;
    [SerializeField]
    private BaseBullet bulletPrefab;
    [SerializeField]
    private LimitedPrefabPool<BaseBullet> bulletPrefabPool;

    private Quaternion _faceCameraRotation;

    void Awake()
    {
        ins = this;
        bulletPrefabPool = new LimitedPrefabPool<BaseBullet>(bulletPrefab, 100, true, "Bullets (Collection)");
    }

    void LateUpdate()
    {
        _faceCameraRotation = mainCameraTransform.rotation;

        for (int i = 0; i < bulletPrefabPool.Actives.Length; i++)
        {
            if (bulletPrefabPool.Actives[i])
            {
                bulletPrefabPool.Objects[i].transform.rotation = _faceCameraRotation;
            }
        }
    }

    public void FireBullet(Vector3 position, Vector3 velocity)
    {
        BaseBullet baseBullet = bulletPrefabPool.Get();
        baseBullet.transform.SetPositionAndRotation(position, _faceCameraRotation);
        baseBullet.Shoot(velocity);
    }

    public void PutBullet(BaseBullet bullet)
    {
        bulletPrefabPool.Put(bullet);
    }
}
