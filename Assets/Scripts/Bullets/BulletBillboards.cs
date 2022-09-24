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

    [SerializeField]
    private BulletType[] bulletTypes;

    private Quaternion _faceCameraRotation;
    public Quaternion FaceCameraRotation => _faceCameraRotation;

    void Awake()
    {
        ins = this;
        CreatePrefabPool();
    }

    void CreatePrefabPool()
    {
#if UNITY_EDITOR
        for (int i = 0; i < bulletTypes.Length; i++)
            bulletTypes[i].InstaintiatePrefabPool(new GameObject("BulletBillboards(Pool)").transform);
#else
        for (int i = 0; i < bulletTypes.Length; i++)
            bulletTypes[i].InstaintiatePrefabPool(null);
#endif
    }

    void LateUpdate()
    {
        _faceCameraRotation = mainCameraTransform.rotation;

        for (int i = 0; i < bulletTypes.Length; i++)
        {
            if (bulletTypes[i].UseBillboardRotate)
            {
                bulletTypes[i].SetBillboardRotation(_faceCameraRotation);
            }
        }
    }
}
