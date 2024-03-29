using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class DroppedItem : MonoBehaviour, IPoolableObj
{
    public static PrefabPool<DroppedItem> Pool {
        get {
            if (s_pool == null) {
                var droppedItem = Resources.Load<DroppedItem>("DroppedItem");
                s_pool = new PrefabPool<DroppedItem>(droppedItem, true, "DroppedItem (Pool)");
            }

            return s_pool;
        }
    }
    private static PrefabPool<DroppedItem> s_pool;


    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private new SphereCollider collider;
    [SerializeField]
    private new Rigidbody rigidbody;
    [SerializeField]
    private float velocityMultiply;

    [SerializeField]
    private Transform rotateTarget;
    [SerializeField]
    private Vector3 spriteRotateSpeed;

    [SerializeField]
    private float delayPutIntoPool = 120;
    private WaitForSeconds _delayWait;
    private Coroutine _putIntoPoolDelay;

    private ItemType _itemType;

    public void Setup(ItemType itemType)
    {
        _itemType = itemType;
        spriteRenderer.sprite = itemType.Sprite;
        Vector3 velocity = Random.onUnitSphere;
        if (velocity.y < 0) velocity.y = -velocity.y;
        rigidbody.velocity = velocity * velocityMultiply;

        _putIntoPoolDelay = StartCoroutine(DelayPutIntoPool());
    }


    public void DeactivateObj(Transform collectionTransform)
    {
        if (_putIntoPoolDelay != null)
        {
            StopCoroutine(_putIntoPoolDelay);
            _putIntoPoolDelay = null;
        }
        gameObject.SetActive(false);
        transform.SetParent(collectionTransform);
    }

    public void Instantiate()
    {
        _delayWait = new WaitForSeconds(delayPutIntoPool);
    }

    public void Reinstantiate()
    {
        gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        rotateTarget.Rotate(spriteRotateSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag(PlayerBehaviour.Tag))
        {
            var playerBehaviour = otherCollider.GetComponent<PlayerBehaviour>();
            playerBehaviour.PickItemUp(_itemType);
            Pool.Put(this);
        }
    }

    IEnumerator DelayPutIntoPool()
    {
        yield return _delayWait;
        _putIntoPoolDelay = null;
        Pool.Put(this);
    }
}
