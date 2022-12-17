using UnityEngine;

namespace XnodeBehaviourTree
{
    public class Context
    {
        public GameObject gameObject;
        public Transform transform;
        public ISlimeBehaviour slimeBehaviour;
        public Animator animator;
        public IAnimationPlayer animationPlayer;

        public Rigidbody rigidbody => _rigidobdy == null ? gameObject.GetComponent<Rigidbody>() : _rigidobdy;
        private Rigidbody _rigidobdy;

        public static Context Create(GameObject gameObject, ISlimeBehaviour _slimeBehaviour)
        {
            // Fetch all commonly used components
            Context context = new Context
            {
                gameObject = gameObject,
                transform = gameObject.transform,
                slimeBehaviour = _slimeBehaviour,
                animator = gameObject.GetComponentInChildren<Animator>(),
                animationPlayer = gameObject.GetComponent<IAnimationPlayer>(),
            };

            return context;
        }
    }
}