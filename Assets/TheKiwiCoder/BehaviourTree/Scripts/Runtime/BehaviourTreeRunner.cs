using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public class BehaviourTreeRunner : MonoBehaviour {

        // The main behaviour tree asset
        public BehaviourTree tree;

        // Storage container object to hold game object subsystems
        protected Context context;

        // Start is called before the first frame update
        protected virtual void Start() {
            context = CreateBehaviourTreeContext();
            tree = tree.Clone();
            tree.Bind(context);
        }

        // Update is called once per frame
        protected virtual void Update() {
            if (tree) {
                tree.Update();
            }
        }

        protected virtual Context CreateBehaviourTreeContext() {
            // return Context.Create(gameObject);
            throw new System.NotImplementedException();
        }

        private void OnDrawGizmosSelected() {
            if (!tree) {
                return;
            }

            for (int i = 0; i < tree.nodes.Count; i++)
            {
                Node node = tree.nodes[i];
                if (node.drawGizmos)
                    node.DrawGizmos(transform);
            }
            // BehaviourTree.Traverse(tree, tree.rootNode, (n) => {
            //     if (n.drawGizmos) {
            //         n.DrawGizmos(transform);
            //     }
            // });
        }
    }
}