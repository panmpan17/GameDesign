using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TheKiwiCoder {
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject {
        public Node rootNode;
        public Node.State treeState = Node.State.Running;
        public List<Node> nodes = new List<Node>();
        public Blackboard blackboard = new Blackboard();

        public Node.State Update() {
            if (rootNode.state == Node.State.Running) {
                treeState = rootNode.Update();
            }
            return treeState;
        }

        public static List<Node> GetChildren(Node parent) {
            if (parent is CompositeNode composite) {
                return composite.children;
            }
            
            List<Node> children = new List<Node>();

            if (parent is DecoratorNode decorator && decorator.child != null) {
                children.Add(decorator.child);
            }
            else if (parent is RootNode rootNode && rootNode.child != null) {
                children.Add(rootNode.child);
            }
            else if (parent is DefineFunctionNode defineFunctionNode && defineFunctionNode.child != null) {
                children.Add(defineFunctionNode.child);
            }

            return children;
        }

        public static void Traverse(Node node, System.Action<Node> visiter) {
            if (node) {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }
        public static void Traverse(BehaviourTree tree, Node node, System.Action<Node> visiter)
        {
            if (node)
            {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(tree, n, visiter));

                if (node is CallFunctionNode)
                {
                    var callNode = (CallFunctionNode)node;
                    Node defineNode = FindFunction(tree, callNode.FunctionName);
                    callNode.SetDefineNode(defineNode);

                    Traverse(tree, defineNode, visiter);
                }
            }
        }

        public static Node FindFunction(BehaviourTree tree, string functionName)
        {
            for (int i = 0; i < tree.nodes.Count; i++)
            {
                if (tree.nodes[i] is DefineFunctionNode && ((DefineFunctionNode)tree.nodes[i]).FunctionName == functionName)
                {
                    return tree.nodes[i];
                }
            }

            return null;
        }

        public BehaviourTree Clone() {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is DefineFunctionNode)
                {
                    Node cloneNode = nodes[i].Clone();
                    Traverse(cloneNode, (n) => {
                        tree.nodes.Add(n);
                    });
                }
            }

            Traverse(tree, tree.rootNode, (n) => {
                tree.nodes.Add(n);
            });

            return tree;
        }

        public void Bind(Context context) {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].context = context;
                nodes[i].blackboard = blackboard;
            }
            // Traverse(this, rootNode, node => {
            //     node.context = context;
            //     node.blackboard = blackboard;
            // });
        }


        #region Editor Compatibility
#if UNITY_EDITOR

        public Node CreateNode(System.Type type) {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            nodes.Add(node);

            if (!Application.isPlaying) {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node) {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
            nodes.Remove(node);

            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child) {
            if (parent is DecoratorNode decorator) {
                Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is RootNode rootNode) {
                Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }

            if (parent is CompositeNode composite) {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }

            if (parent is DefineFunctionNode defineFunction) {
                Undo.RecordObject(defineFunction, "Behaviour Tree (AddChild)");
                defineFunction.child = child;
                EditorUtility.SetDirty(defineFunction);
            }
        }

        public void RemoveChild(Node parent, Node child) {
            if (parent is DecoratorNode decorator) {
                Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is RootNode rootNode) {
                Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
            }

            if (parent is CompositeNode composite) {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }

            if (parent is DefineFunctionNode defineFunction) {
                Undo.RecordObject(defineFunction, "Behaviour Tree (RemoveChild)");
                defineFunction.child = null;
                EditorUtility.SetDirty(defineFunction);
            }
        }
#endif
        #endregion Editor Compatibility
    }
}