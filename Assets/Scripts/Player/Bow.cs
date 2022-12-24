using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using MPack;
using Cinemachine;

public class Bow : MonoBehaviour
{
    [SerializeField]
    private Transform arrowPlacePoint;
    [SerializeField]
    private Arrow arrowPrefab;
    private LimitedPrefabPool<Arrow> _arrowPrefabPool;
    [SerializeField]
    private CinemachineImpulseSource impulseSource;
    [SerializeField]
    private TransformPointer currentArrowPointer;
    [SerializeField]
    private EventReference aimProgressEvent;

    [SerializeField]
    private List<BowParameter> bowParameters;
    private BowParameter _currentParameter;
    public BowParameter CurrentParameter => _currentParameter;

    private int _walkingCameraIndex;
    private int _aimCameraIndex;
    private int _deepAimCameraIndex;

    private bool _extraAimStarted = false;
    private Stopwatch _extraAimStopWatch = new Stopwatch();
    private PlayerBehaviour _playerBehaviour;

    public Arrow PreparedArrow { get; private set; }


    void Awake()
    {
        _arrowPrefabPool = new LimitedPrefabPool<Arrow>(arrowPrefab, 10, true, "Arrows (Pool)");

        _walkingCameraIndex = CameraSwitcher.GetCameraIndex("Walk");
        _deepAimCameraIndex = CameraSwitcher.GetCameraIndex("DeepAim");
        _aimCameraIndex = CameraSwitcher.GetCameraIndex("Aim");

        _currentParameter = ScriptableObject.CreateInstance<BowParameter>();
        _currentParameter.CombineParamaters(bowParameters);
        // _currentParameter.LogParamaters();
    }

    public void Setup(PlayerBehaviour playerBehaviour)
    {
        _playerBehaviour = playerBehaviour;
    }

    public void OnAimDown()
    {
        if (PreparedArrow == null)
        {
            PreparedArrow = _arrowPrefabPool.Get();
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
            float extraProgress = Mathf.Min(_extraAimStopWatch.DeltaTime / _currentParameter.SecondDrawDuration, 1);

            PreparedArrow.transform.SetParent(null);

            float speed = _currentParameter.ArrowSpeed.Lerp(extraProgress);

            Vector3 normalizedDelta = (rayHitPosition - PreparedArrow.transform.position).normalized;
            float ignoreGravityDuration = _currentParameter.IgnoreGravityTime + (_currentParameter.SecondDrawExtendIgnoreGravityTime * extraProgress);
            PreparedArrow.Shoot(normalizedDelta * speed, ignoreGravityDuration, extraProgress);
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

        float extraProgress = Mathf.Min(_extraAimStopWatch.DeltaTime / _currentParameter.SecondDrawDuration, 1);
        aimProgressEvent.Invoke(1 + extraProgress);
    }

    IEnumerator SwitchCamera()
    {
        CameraSwitcher.ins.SwitchTo(_aimCameraIndex);
        yield return new WaitForSeconds(0.1f);
        CameraSwitcher.ins.SwitchTo(_walkingCameraIndex);
    }

    public void UpgradeBow(BowParameter upgradeBowParameter)
    {
        bowParameters.Add(upgradeBowParameter);
        _currentParameter.CombineParamaters(bowParameters);
        // _currentParameter.LogParamaters();
    }

    public void DisableAllArrows()
    {
        _arrowPrefabPool.DisableAll();
    }
}
