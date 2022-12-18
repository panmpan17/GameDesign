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
    [SerializeField]
    private LayerMaskReference groundLayers;

    [SerializeField]
    private JumpForawrdNode jumpForwardNode;

    private int _stage = 0;

    private SlimeBehaviour _slimeBehaviour;
    private BehaviourTreeRunner _behaviourTreeRunner;
    private SlimeAnimationController _animator;

    private ArrowBounceOff[] bounceOffs;

    void Awake()
    {
        _slimeBehaviour = GetComponent<SlimeBehaviour>();
        _behaviourTreeRunner = GetComponent<BehaviourTreeRunner>();
        _animator = GetComponent<SlimeAnimationController>();

        _slimeBehaviour.OnCoreDamagedEvent += OnCoreDamaged;
        _slimeBehaviour.OnBounceOffEvent += OnArrowBounceOff;

        bounceOffs = GetComponentsInChildren<ArrowBounceOff>();
        foreach (var bounceOff in bounceOffs) bounceOff.OnBounceOffEvent += OnArrowBounceOff;

        foreach (SlimeCore core in stage2Cores) core.enabled = false;
        foreach (SlimeCore core in stage3Cores) core.enabled = false;
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
                {
                    ReleaseObject(stage1EndCloseObjects);
                    foreach (SlimeCore core in stage2Cores) core.enabled = true;
                }
                break;

            case 1:
                for (int i = 0; i < stage2Cores.Length && allDestoried; i++)
                {
                    if (stage2Cores[i].gameObject.activeSelf)
                        allDestoried = false;
                }

                if (allDestoried)
                {
                    ReleaseObject(stage2EndCloseObjects);
                    foreach (SlimeCore core in stage3Cores) core.enabled = true;
                }
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

        if (_rigidbody.GetComponent<Collider>() is var collider && collider)
        {
            collider.enabled = true;
            collider.isTrigger = false;
            if (collider is MeshCollider) ((MeshCollider)collider).convex = true;
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

#region Boss open scene
    public void AwakeFromSleep()
    {
        _animator.SwitchToAnimation("Blink1");
    }

    public IEnumerator RotateAndJump(Vector3 position)
    {
        yield return StartCoroutine(RotateToFacePosition(position));
        yield return StartCoroutine(Jump());
    }

    IEnumerator RotateToFacePosition(Vector3 position)
    {
        Quaternion destinationRotation = Quaternion.LookRotation(
            position - transform.position, transform.up);

        Vector3 origin = transform.position + (destinationRotation * (Vector3.forward * 0.1f));
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 2, groundLayers.Value))
        {
            destinationRotation = Quaternion.LookRotation(hit.point - transform.position, transform.up);
        }

        while (true)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, destinationRotation, 100 * Time.deltaTime);
            float angleDifference = Quaternion.Angle(transform.rotation, destinationRotation);
            yield return null;

            if (angleDifference < 0.01f)
                break;
        }
    }

    IEnumerator Jump()
    {
        Context context = Context.Create(gameObject, _slimeBehaviour);
        JumpForawrdNode node = (JumpForawrdNode)jumpForwardNode.Clone();
        node.context = context;
        AbstractBehaviourNode.State state = node.Update();

        while (state == AbstractBehaviourNode.State.Running)
        {
            state = node.Update();
            yield return null;
        }
        Debug.Log(state);
    }

    public void EnableBehaviourTreeRuner()
    {
        _behaviourTreeRunner.enabled = true;
    }

    void OnArrowBounceOff()
    {
        _animator.SwitchToAnimation("SleepAwake");
        _animator.SetOneTimeDestinateAnimation("Sleep");
    }
#endregion
}
