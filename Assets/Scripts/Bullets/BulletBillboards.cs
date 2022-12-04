using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class BulletBillboards : MonoBehaviour
{
    public static BulletBillboards ins;

    [SerializeField]
    private TransformPointer cameraTransform;
    private Transform _mainCameraTransform;
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

        cameraTransform.OnChange += ChangeCameraTransform;
        if (cameraTransform.Target) ChangeCameraTransform(cameraTransform.Target);
    }

    void ChangeCameraTransform(Transform cameraTransform) => _mainCameraTransform = cameraTransform;

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
        if (!_mainCameraTransform)
            return;

        _faceCameraRotation = _mainCameraTransform.rotation;

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

    void OnDestroy()
    {
        playerReviveEvent.InvokeEvents -= ResetBullets;
        cameraTransform.OnChange -= ChangeCameraTransform;
    }
}
