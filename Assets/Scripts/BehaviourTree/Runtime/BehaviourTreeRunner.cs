// using System;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace XnodeBehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField]
        private BehaviourTreeGraph graph;

        private Blackboard _blackboard;

        void Start()
        {
            Context context = Context.Create(gameObject, GetComponent<ISlimeBehaviour>());
            _blackboard = new Blackboard();

            graph = (BehaviourTreeGraph) graph.Copy();
            graph.OnInitial(context, _blackboard);
        }

        void Update()
        {
            graph.Update();
        }

        public void SetStage(int stageIndex)
        {
            _blackboard.Stage = stageIndex;
        }
    }
}