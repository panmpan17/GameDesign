using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName="Game/GameObject Pool")]
public class GameObjectPoolReference : ScriptableObject
{
    public GameObject Prefab;
    public bool CreateCollection;
    public string CollectionName;

    [System.NonSerialized]
    private GameObjectPrefabPool _pool;

    public void CreatePool()
    {
        _pool = new GameObjectPrefabPool(
            Prefab,
            CreateCollection,
            CollectionName);
    }
    public void ClearPool() => _pool = null;

    public void PutAllAliveObjects() => _pool.PutAllAliveObjects();

    public GameObject Get() =>_pool.Get();
    public void Put(GameObject target) => _pool.Put(target);
}
