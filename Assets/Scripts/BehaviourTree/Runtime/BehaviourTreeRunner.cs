using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField]
        private BehaviourTreeGraph graph;

        void Awake()
        {
            graph = graph.Clone();
        }

        void Update()
        {
            graph.Update();
        }
    }
}