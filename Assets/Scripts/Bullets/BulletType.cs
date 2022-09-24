using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName = "Game/子彈類型")]
public class BulletType : ScriptableObject
{
    public GameObject Prefab;
    public bool AllowBulletTrigger;
    public bool AllowCanonPhysic;

    public bool UseBillboardRotate;

    public int InstantiateCount;

    private LimitedPrefabPool<BulletBehaviour> _pool;
    public LimitedPrefabPool<BulletBehaviour> Pool => _pool;

    public void InstaintiatePrefabPool(Transform collectionTransform)
    {
        _pool = new LimitedPrefabPool<BulletBehaviour>(
            Prefab.GetComponent<BulletBehaviour>(),
            InstantiateCount,
            collectionTransform: collectionTransform);
        
        for (int i = 0; i < _pool.Objects.Length; i++)
            _pool.Objects[i].putFunction = _pool.Put;
    }

    public void SetBillboardRotation(Quaternion rotation)
    {
        for (int i = 0; i < _pool.Actives.Length; i++)
        {
            if (_pool.Actives[i])
            {
                _pool.Objects[i].transform.rotation = rotation;
            }
        }
    }
}
