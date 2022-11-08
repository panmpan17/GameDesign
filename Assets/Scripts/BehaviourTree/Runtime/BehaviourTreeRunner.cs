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
            graph = (BehaviourTreeGraph) graph.Copy();
        }

        void Update()
        {
            graph.Update();
        }
    }
}