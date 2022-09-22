using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder {

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context {
        public GameObject gameObject;
        public Transform transform;
        public SlimeBehaviourTreeRunner slimeBehaviour;
        public Animator animator;

        public Rigidbody rigidbody => _rigidobdy == null ? gameObject.GetComponent<Rigidbody>() : _rigidobdy;
        private Rigidbody _rigidobdy;

        public static Context Create(SlimeBehaviourTreeRunner _slimeBehaviour)
        {
            // Fetch all commonly used components
            Context context = new Context {
                gameObject = _slimeBehaviour.gameObject,
                transform = _slimeBehaviour.transform,
                slimeBehaviour = _slimeBehaviour,
                animator = _slimeBehaviour.GetComponentInChildren<Animator>(),
            };

            return context;
        }
    }
}