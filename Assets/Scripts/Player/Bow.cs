using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using Cinemachine;

public class Bow : MonoBehaviour
{
    [SerializeField]
    private Transform arrowPlacePoint;
    [SerializeField]
    private Arrow arrowPrefab;
    [SerializeField]
    private LimitedPrefabPool<Arrow> arrowPrefabPool;
    [SerializeField]
    private CinemachineImpulseSource impulseSource;
    [SerializeField]
    private TransformPointer currentArrowPointer;
    [SerializeField]
    private EventReference aimProgressEvent;

    [SerializeField]
    private float extraAimTime;

    private int _walkingCameraIndex;
    private int _aimCameraIndex;
    private int _deepAimCameraIndex;

    private bool _extraAimStarted = false;
    private Stopwatch _extraAimStopWatch = new Stopwatch();
    private PlayerBehaviour _playerBehaviour;

    public Arrow PreparedArrow { get; private set; }


    void Awake()
    {
        arrowPrefabPool = new LimitedPrefabPool<Arrow>(arrowPrefab, 10, true, "Arrows (Pool)");

        _walkingCameraIndex = CameraSwitcher.GetCameraIndex("Walk");
        _deepAimCameraIndex = CameraSwitcher.GetCameraIndex("DeepAim");
        _aimCameraIndex = CameraSwitcher.GetCameraIndex("Aim");
    }

    public void Setup(PlayerBehaviour playerBehaviour)
    {
        _playerBehaviour = playerBehaviour;
    }

    public void OnAimDown()
    {
        if (PreparedArrow == null)
        {
            PreparedArrow = arrowPrefabPool.Get();
            Transform arrowTransform = PreparedArrow.transform;
            arrowTransform.SetParent(arrowPlacePoint);
            arrowTransform.localPosition = Vector3.zero;
            arrowTransform.localRotation = Quaternion.identity;
        }
        else
            PreparedArrow.gameObject.SetActive(true);

        currentArrowPointer.Target = PreparedArrow.transform;

        CameraSwitcher.ins.SwitchTo(_aimCameraIndex);
    }

    public void OnAimUp(bool isBowFullyDrawed, Vector3 rayHitPosition)
    {
        _extraAimStarted = false;
        aimProgressEvent.Invoke(0f);

        if (isBowFullyDrawed)
        {
            float extraProgress = Mathf.Min(_extraAimStopWatch.DeltaTime / extraAimTime, 1);

            PreparedArrow.transform.SetParent(null);
            PreparedArrow.Shoot(rayHitPosition, extraProgress);
            PreparedArrow = null;

            _playerBehaviour.TriggerBowShoot(extraProgress);

            impulseSource.GenerateImpulse(extraProgress);
            StartCoroutine(SwitchCamera());
        }
        else
        {
            PreparedArrow.gameObject.SetActive(false);
            CameraSwitcher.ins.SwitchTo(_walkingCameraIndex);
        }
    }

    public void OnAimProgress(float progress)
    {
        if (progress < 1)
        {
            aimProgressEvent.Invoke(progress);
            return;
        }

        if (!_extraAimStarted)
        {
            _extraAimStarted = true;
            _extraAimStopWatch.Update();
            CameraSwitcher.ins.SwitchTo(_deepAimCameraIndex);
        }

        float extraProgress = Mathf.Min(_extraAimStopWatch.DeltaTime / extraAimTime, 1);
        aimProgressEvent.Invoke(1 + extraProgress);
    }

    IEnumerator SwitchCamera()
    {
        CameraSwitcher.ins.SwitchTo(_aimCameraIndex);
        yield return new WaitForSeconds(0.1f);
        CameraSwitcher.ins.SwitchTo(_walkingCameraIndex);
    }
}
