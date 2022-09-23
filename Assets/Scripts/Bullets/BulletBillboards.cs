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
    private int bulletInitialCount;
    private LimitedPrefabPool<BaseBullet> bulletPrefabPool;

    [SerializeField]
    private CanonShell canonShellPrefab;
    [SerializeField]
    private int canonShellInitialCount;
    private LimitedPrefabPool<CanonShell> canonShellPrefabPool;

#if UNITY_EDITOR
    private Transform poolColletionTransform;
#endif

    private Quaternion _faceCameraRotation;

    void Awake()
    {
        ins = this;
        CreatePrefabPool();
    }

    void CreatePrefabPool()
    {
#if UNITY_EDITOR
        GameObject newObject = new GameObject("BulletBillboards(Pool)");
        poolColletionTransform = newObject.transform;

        bulletPrefabPool = new LimitedPrefabPool<BaseBullet>(bulletPrefab, bulletInitialCount, collectionTransform: poolColletionTransform);
        canonShellPrefabPool = new LimitedPrefabPool<CanonShell>(canonShellPrefab, canonShellInitialCount, collectionTransform: poolColletionTransform);
#else
        bulletPrefabPool = new LimitedPrefabPool<BaseBullet>(bulletPrefab, 100);
        canonShellPrefabPool = new LimitedPrefabPool<CanonShell>(canonShellPrefab, canonShellInitialCount, collectionTransform: poolColletionTransform);
#endif
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

        for (int i = 0; i < canonShellPrefabPool.Actives.Length; i++)
        {
            if (canonShellPrefabPool.Actives[i])
            {
                canonShellPrefabPool.Objects[i].transform.rotation = _faceCameraRotation;
            }
        }
    }

    public void FireBullet(Vector3 position, Vector3 velocity)
    {
        BaseBullet baseBullet = bulletPrefabPool.Get();
        baseBullet.transform.SetPositionAndRotation(position, _faceCameraRotation);
        baseBullet.Shoot(velocity);
    }

    public void FireCanonShell(PhysicSimulate physicSimulate)
    {
        CanonShell shell = canonShellPrefabPool.Get();
        shell.Setup(physicSimulate);
    }

    public void PutBullet(BaseBullet bullet) => bulletPrefabPool.Put(bullet);
    public void PutCanonShell(CanonShell canon) => canonShellPrefabPool.Put(canon);
}
