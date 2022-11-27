using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack {
    public interface IPoolableObj {
        void Instantiate();
        void DeactivateObj(Transform collectionTransform);
        void Reinstantiate();
    }

    public class PrefabPool<T> where T: MonoBehaviour, IPoolableObj
    {
        public T Prefab;

        public delegate T PrefabInstantiateFunc();
        public PrefabInstantiateFunc InstantiateFunc;

        public List<T> AliveObjs, PoolObjs;
        public Transform PoolCollection;

        public PrefabPool(T prefab, bool createPoolCollection=false, string poolCollectionName="Pool Collection")
        {
            Prefab = prefab;
            InstantiateFunc = null;
            AliveObjs = new List<T>();
            PoolObjs = new List<T>();

            if (createPoolCollection)
            {
                GameObject obj = new GameObject(poolCollectionName);
                PoolCollection = obj.transform;
            }
        }

        public PrefabPool(PrefabInstantiateFunc instantiateFunc, bool createPoolCollection=false, string poolCollectionName="Pool Collection")
        {
            Prefab = null;
            InstantiateFunc = instantiateFunc;
            AliveObjs = new List<T>();
            PoolObjs = new List<T>();

            if (createPoolCollection)
            {
                GameObject obj = new GameObject(poolCollectionName);
                PoolCollection = obj.transform;
            }
        }

        public void ClearAliveObjects()
        {
            for (int i = 0; i < AliveObjs.Count; i++)
            {
                GameObject.Destroy(AliveObjs[i]);
            }
            AliveObjs.Clear();
        }

        public void ClearPoolObjects()
        {
            for (int i = 0; i < PoolObjs.Count; i++)
            {
                GameObject.Destroy(PoolObjs[i]);
            }
            PoolObjs.Clear();
        }

        public void ClearObjects()
        {
            for (int i = 0; i < AliveObjs.Count; i++)
            {
                GameObject.Destroy(AliveObjs[i]);
            }
            for (int i = 0; i < PoolObjs.Count; i++)
            {
                GameObject.Destroy(PoolObjs[i]);
            }
            AliveObjs.Clear();
            PoolObjs.Clear();
        }

        public T Get()
        {
            T component;
            if (PoolObjs.Count > 0) {
                component = PoolObjs[0];
                PoolObjs.RemoveAt(0);

                component.Reinstantiate();
            }
            else {
                if (Prefab == null)
                    component = InstantiateFunc.Invoke();
                else
                    component = GameObject.Instantiate(Prefab);

                component.Instantiate();
            }

            AliveObjs.Add(component);

            return component;
        }

        public void Put(T component)
        {
            component.DeactivateObj(PoolCollection);
            AliveObjs.Remove(component);
            PoolObjs.Add(component);
        }
    }


    public class LimitedPrefabPool<T> where T : MonoBehaviour, IPoolableObj
    {
        public T Prefab;

        public delegate T PrefabInstantiateFunc();

        public T[] Objects;
        public bool[] Actives;
        private int _index;

        public Transform PoolCollection;

        public LimitedPrefabPool(T prefab, int initialCount,
            bool createPoolCollection=false, string poolCollectionName="Pool Collection",
            Transform collectionTransform=null)
        {
            Prefab = prefab;
            Objects = new T[initialCount];
            Actives = new bool[initialCount];

            if (collectionTransform != null)
            {
                PoolCollection = collectionTransform;
            }
            else if (createPoolCollection)
            {
                GameObject obj = new GameObject(poolCollectionName);
                PoolCollection = obj.transform;
            }

            for (int i = 0; i < initialCount; i++)
            {
                T newObject = GameObject.Instantiate(Prefab);
                Objects[i] = newObject;

                newObject.Instantiate();
                newObject.DeactivateObj(PoolCollection);
            }
        }

        public LimitedPrefabPool(PrefabInstantiateFunc instantiateFunc, int initialCount, bool createPoolCollection = false, string poolCollectionName = "Pool Collection")
        {
            Prefab = null;
            Objects = new T[initialCount];
            Actives = new bool[initialCount];

            if (createPoolCollection)
            {
                GameObject obj = new GameObject(poolCollectionName);
                PoolCollection = obj.transform;
            }

            for (int i = 0; i < initialCount; i++)
            {
                T newObject = instantiateFunc.Invoke();
                Objects[i] = newObject;

                newObject.Instantiate();
                newObject.DeactivateObj(PoolCollection);
            }
        }

        public T Get()
        {
            T component = Objects[_index];

            if (Actives[_index])
                component.DeactivateObj(PoolCollection);
            Actives[_index] = true;

            component.Reinstantiate();

            if (++_index >= Actives.Length)
                _index = 0;

            return component;
        }

        public void Put(T component)
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] == component)
                {
                    component.DeactivateObj(PoolCollection);
                    Actives[i] = false;
                    return;
                }
            }

#if UNITY_EDITOR
            throw new System.ArgumentException("Component doesn't belong to this prefab pool");
#endif
        }

        public void DisableAll()
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                Objects[i].DeactivateObj(PoolCollection);
                Actives[i] = false;
            }
        }
    }


    [System.Serializable]
    public class GameObjectPrefabPool
    {
        public GameObject Prefab = null;

        public delegate GameObject PrefabInstantiateFunc();
        public PrefabInstantiateFunc InstantiateFunc = null;

        [System.NonSerialized]
        public List<GameObject> AliveObjs = new List<GameObject>(), PoolObjs =  new List<GameObject>();
        public Transform PoolCollection;

        public int InitialObjCount;

        public GameObjectPrefabPool(GameObject prefab, bool createPoolCollection = false, string poolCollectionName = "Pool Collection")
        {
            Prefab = prefab;
            AliveObjs = new List<GameObject>();
            PoolObjs = new List<GameObject>();

            if (createPoolCollection)
            {
                GameObject obj = new GameObject(poolCollectionName);
                PoolCollection = obj.transform;
            }
        }

        public GameObjectPrefabPool(PrefabInstantiateFunc instantiateFunc, bool createPoolCollection = false, string poolCollectionName = "Pool Collection")
        {
            InstantiateFunc = instantiateFunc;
            AliveObjs = new List<GameObject>();
            PoolObjs = new List<GameObject>();

            if (createPoolCollection)
            {
                GameObject obj = new GameObject(poolCollectionName);
                PoolCollection = obj.transform;
            }
        }

        public void Initial()
        {
            if (AliveObjs == null) AliveObjs = new List<GameObject>();
            if (PoolObjs == null) PoolObjs = new List<GameObject>();

            for (int i = 0; i < InitialObjCount; i++)
                Get();

            while (AliveObjs.Count > 0)
                Put(AliveObjs[0]);
        }

        public void ClearAliveObjs()
        {
            while (AliveObjs.Count > 0)
            {
                GameObject.Destroy(AliveObjs[0]);
                AliveObjs.RemoveAt(0);
            }
        }

        public void ClearPoolObjs()
        {
            while (PoolObjs.Count > 0)
            {
                GameObject.Destroy(PoolObjs[0]);
                PoolObjs.RemoveAt(0);
            }
        }

        public GameObject Get()
        {
            GameObject obj;
            if (PoolObjs.Count > 0)
            {
                obj = PoolObjs[0];
                PoolObjs.RemoveAt(0);
            }
            else
            {
                if (Prefab == null)
                    obj = InstantiateFunc.Invoke();
                else
                    obj = GameObject.Instantiate(Prefab);
            }

            AliveObjs.Add(obj);
            obj.SetActive(true);

            return obj;
        }

        public GameObject GetRecursivly(int count, Vector3 localPosOffset)
        {
            if (count == 1)
                return Get();
            else if (count <= 0)
                throw new System.ArgumentException("Count is zero");

            GameObject parent = Get();
            Transform child = GetRecursivly(count - 1, localPosOffset).transform;
            child.SetParent(parent.transform);
            child.localPosition = localPosOffset;
            child.localScale = Vector3.one;
            child.localRotation = Quaternion.identity;

            return parent;
        }

        public void Put(GameObject obj)
        {
            AliveObjs.Remove(obj);
            PoolObjs.Add(obj);
            obj.SetActive(false);
            obj.transform.SetParent(PoolCollection);
        }

        public void PutRecursivly(Transform objT)
        {
            if (objT.childCount > 0)
                PutRecursivly(objT.GetChild(0));
            Put(objT.gameObject);
        }

        public void PutAllAliveObjects()
        {
            PoolObjs.AddRange(AliveObjs);

            foreach (GameObject aliveObject in AliveObjs)
            {
                aliveObject.SetActive(false);
                aliveObject.transform.SetParent(PoolCollection);
            }

            AliveObjs.Clear();
        }
    }
}