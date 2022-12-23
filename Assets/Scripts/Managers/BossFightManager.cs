using System.Collections;
using UnityEngine;
using Cinemachine;

public class BossFightManager : MonoBehaviour
{
    [Header("Other Refernce")]
    [SerializeField]
    private PlayerDetection entranceDetect;
    [SerializeField]
    private GameObject borderWall;
    [SerializeField]
    private BGMClip bgmClip;
    [SerializeField]
    private Animator blackBarAnimatior;

    [Header("Player")]
    [SerializeField]
    private EventReference playerReviveEvent;

    [Header("Camera")]
    [SerializeField]
    private CinemachineImpulseSource impulseSource;
    [SerializeField]
    private CinemachineVirtualCamera lookBossCam;
    [SerializeField]
    private string switchCamera = "BossLand";
    [SerializeField]
    private float switchCameraWait = 0.5f;
    [SerializeField]
    private float switchBackCameraDelay = 4f;

    [Header("Boss Slime")]
    [SerializeField]
    private BossSlime bossSlime;
    private BossSlime _bossSlimePrefab;

    [SerializeField]
    private EventReference slimeHealthShowEvent;
    [SerializeField]
    private GameObjectList bossSpawnedSlimes;

    // TODO: Cutscene, able to skip when player is dead or skip all cutscene

    [Header("Cutscene")]
    [SerializeField]
    private GameObject endingCutscene;
    [SerializeField]
    private float endingCutsceneDelay;
    [SerializeField]
    private float cutsceneDuration;

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool bossInstantDeath;
#endif

    void Awake()
    {
        borderWall.SetActive(false);
        entranceDetect.OnPlayerEnterEvent += OnPlayerEnterEntrance;

        _bossSlimePrefab = Instantiate<BossSlime>(bossSlime);
        _bossSlimePrefab.gameObject.SetActive(false);
    }

    void OnPlayerEnterEntrance()
    {
        playerReviveEvent.InvokeEvents += ResetBossFight;

        entranceDetect.gameObject.SetActive(false);
        borderWall.SetActive(true);

        BGMPlayer.ins.BlendNewBGM(bgmClip);

        StartCoroutine(C_BossEnterEvent());
    }

    IEnumerator C_BossEnterEvent()
    {
        AudioVolumeControl.ins.FadeOutEnvironmentVolume();

        var HUD = GameObject.Find("HUD").GetComponent<PlayerStatusHUD>();
        HUD.FadeOut(1f);
        blackBarAnimatior.gameObject.SetActive(true);

        InputInterface playerInput = GameManager.ins.Player.Input;
        playerInput.Disable();

        CameraSwitcher.ins.SwitchTo(switchCamera);
        lookBossCam.LookAt = bossSlime.transform;
        yield return new WaitForSeconds(switchCameraWait);

        bossSlime.AwakeFromSleep();
        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(bossSlime.RotateAndJump(GameManager.ins.Player.transform.position));

        var slimeBehaviour = bossSlime.GetComponent<SlimeBehaviour>();

        slimeBehaviour.TriggerImpluse(0.1f);
        impulseSource.GenerateImpulse();
        yield return new WaitForSeconds(switchBackCameraDelay);


        CameraSwitcher.ins.SwitchTo("Walk");

        yield return new WaitForSeconds(switchCameraWait - 1);
        HUD.FadeIn(1f);
        blackBarAnimatior.SetTrigger("FadeOut");

        yield return new WaitForSeconds(1);
        blackBarAnimatior.gameObject.SetActive(true);
        bossSlime.EnableBehaviourTreeRuner();

        playerInput.Enable();
        slimeHealthShowEvent.Invoke(true);

        slimeBehaviour.UpdateHealth();
        slimeBehaviour.OnDeath.AddListener(OnBossDeath);

#if UNITY_EDITOR
        if (bossInstantDeath)
        {
            slimeBehaviour.DisableTreeRunner();
            OnBossDeath();
        }
#endif
    }

    void ResetBossFight()
    {
        playerReviveEvent.InvokeEvents -= ResetBossFight;

        borderWall.SetActive(false);
        slimeHealthShowEvent.Invoke(false);
        Destroy(bossSlime.gameObject);
        bossSlime = Instantiate<BossSlime>(_bossSlimePrefab);
        bossSlime.gameObject.SetActive(true);

        // Kill all small slime that spawn by boss
        bossSpawnedSlimes?.DestroyAll();
        PrefabPoolManager.ins.PutAllAliveObjects();

        entranceDetect.gameObject.SetActive(true);

        BGMPlayer.ins.ResetToBaseBGM();
        AudioVolumeControl.ins.FadeInEnvironmentVolume();

        DroppedItem.Pool.PutAllAliveObjects();
    }

    void OnBossDeath()
    {
        StartCoroutine(StartEnding());
    }

    IEnumerator StartEnding()
    {
        GameManager.ins.Player.SetInvincible(true);
        yield return new WaitForSeconds(endingCutsceneDelay);

        GameManager.ins.Player.Input.Disable();
        
        // GameManager.ins.Player.enabled = false;

        // Time.timeScale = 1;
        GameObject ending = Instantiate(endingCutscene);

        yield return new WaitForSeconds(cutsceneDuration);
        LoadScene.ins.Load("MainMenu");
        // Time.timeScale = 1;
    }
}
