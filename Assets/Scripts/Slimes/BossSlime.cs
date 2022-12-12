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
                {
                    for (int i = 0; i < stage1EndCloseObjects.Length; i++)
                        stage1EndCloseObjects[i].SetActive(false);
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
                    for (int i = 0; i < stage2EndCloseObjects.Length; i++)
                        stage2EndCloseObjects[i].SetActive(false);
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

    void OnDestroy()
    {
        _slimeBehaviour.OnCoreDamagedEvent -= OnCoreDamaged;
    }
}
