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

        void Start()
        {
            TheKiwiCoder.Context context = TheKiwiCoder.Context.Create(gameObject, GetComponent<ISlimeBehaviour>());
            graph = (BehaviourTreeGraph) graph.Copy();
            graph.OnInitial(context);
        }

        void Update()
        {
            graph.Update();
        }
    }
}