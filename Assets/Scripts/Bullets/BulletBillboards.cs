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

    [SerializeField]
    private EventReference playerReviveEvent;

    private Quaternion _faceCameraRotation;
    public Quaternion FaceCameraRotation => _faceCameraRotation;

    void Awake()
    {
        ins = this;
        CreatePrefabPool();
        playerReviveEvent.InvokeEvents += ResetBullets;

        if (mainCameraTransform == null) mainCameraTransform = Camera.main.transform;
    }

    void CreatePrefabPool()
    {
#if UNITY_EDITOR
        Transform pool = new GameObject("BulletBillboards(Pool)").transform;
        for (int i = 0; i < bulletTypes.Length; i++)
            bulletTypes[i].InstaintiatePrefabPool(pool);
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

    void ResetBullets()
    {
        for (int i = 0; i < bulletTypes.Length; i++)
        {
            bulletTypes[i].Pool.DisableAll();
        }
    }
}
