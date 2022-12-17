using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using XnodeBehaviourTree;


[RequireComponent(typeof(SlimeBehaviour))]
public class BossSlime : MonoBehaviour
{
    [SerializeField]
    private SlimeCore[] stage1Cores;
    [SerializeField]
    private GameObject[] stage1EndCloseObjects;

    [SerializeField]
    private SlimeCore[] stage2Cores;
    [SerializeField]
    private GameObject[] stage2EndCloseObjects;

    [SerializeField]
    private SlimeCore[] stage3Cores;

    [SerializeField]
    [Layer]
    private int layer;

    private int _stage = 0;

    private SlimeBehaviour _slimeBehaviour;
    private BehaviourTreeRunner _behaviourTreeRunner;

    void Awake()
    {
        _slimeBehaviour = GetComponent<SlimeBehaviour>();
        _slimeBehaviour.OnCoreDamagedEvent += OnCoreDamaged;

        _behaviourTreeRunner = GetComponent<BehaviourTreeRunner>();
    }

    void OnCoreDamaged(SlimeCore slimeCore)
    {
        bool allDestoried = true;
        switch (_stage)
        {
            case 0:
                for (int i = 0; i < stage1Cores.Length && allDestoried; i++)
                {
                    if (stage1Cores[i].gameObject.activeSelf)
                        allDestoried = false;
                }

                if (allDestoried)
                    ReleaseObject(stage1EndCloseObjects);
                break;

            case 1:
                for (int i = 0; i < stage2Cores.Length && allDestoried; i++)
                {
                    if (stage2Cores[i].gameObject.activeSelf)
                        allDestoried = false;
                }

                if (allDestoried)
                    ReleaseObject(stage2EndCloseObjects);
                break;

            case 2:
                allDestoried = false;
                break;
        }

        if (allDestoried)
        {
            _stage++;
            _behaviourTreeRunner.SetStage(_stage);
        }
    }

    void ReleaseObject(GameObject[] objects)
    {
        foreach (GameObject _gameObject in objects)
        {
            Rigidbody rigidbody = _gameObject.GetComponent<Rigidbody>();
            if (!rigidbody)
            {
                _gameObject.SetActive(false);
                continue;
            }

            StartCoroutine(DetachObject(rigidbody, 3f));
        }
        GetComponentInChildren<Animator>().Rebind();
    }

    IEnumerator DetachObject(Rigidbody _rigidbody, float duration)
    {
        _rigidbody.gameObject.layer = layer;

        Transform _transform = _rigidbody.gameObject.transform;
        Vector3 position = _transform.position;
        Vector3 scale = _transform.lossyScale;
        Quaternion rotation = _transform.rotation;
        _transform.SetParent(null, true);

        var collider = _rigidbody.GetComponent<MeshCollider>();
        if (collider) collider.convex = true;
        else
        {
            _rigidbody.GetComponent<Collider>().isTrigger = false;
        }

        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;

        Vector3 _velocity = _transform.position - transform.position;
        _velocity.y += 1;
        _velocity.Normalize();
        _rigidbody.velocity = _velocity * 5;

        yield return new WaitForEndOfFrame();

        _transform.SetPositionAndRotation(position, rotation);
        _transform.localScale = scale * 0.9f;

        yield return new WaitForSeconds(duration);
        _rigidbody.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        _slimeBehaviour.OnCoreDamagedEvent -= OnCoreDamaged;
    }
}
