using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MPack
{
    public class DialogueGraph : NodeGraph
    {
        public VariablePair[] variablePairs;

        public AbstractNode currentNode { get; protected set; }

        public IDialogueInterpreter DialogueInterpreter { get; private set; }

        [SerializeField]
        private VariableStorage varibleStorage;
        public VariableStorage varibles => varibleStorage;

        public void SetUp(IDialogueInterpreter dialogueInterpreter)
        {
            DialogueInterpreter = dialogueInterpreter;
        }

        public void TearDown()
        {
            DialogueInterpreter = null;
        }

        public void Start()
        {
            for  (int i = 0; i < nodes.Count; i++)
            {
                try
                {
                    StartNode startNode = (StartNode)nodes[i];
                    currentNode = startNode;
                    return;
                }
                catch(InvalidCastException) {}
            }
        }

        public void JumpToNode(AbstractNode node)
        {
            currentNode = node;
        }

        public void JumpToNextNode()
        {
            NodePort port = currentNode.GetOutputPort("Output");

            if (port.IsConnected)
            {
                Node node = port.Connection.node;
                currentNode = (AbstractNode)node;
            }
            else
            {
                currentNode = null;
            }
        }

        public void Proccessing()
        {
            while (true)
            {
                if (currentNode == null)
                {
                    DialogueInterpreter.OnDialogueEnd();
                    return;
                }

                currentNode.Proccess();
                // Debug.Log(currentNode, currentNode);
                // Debug.Log(currentNode.status);
                // Debug.Log(currentNode.nextNode, currentNode.nextNode);

                switch (currentNode.status)
                {
                    case AbstractNode.Status.Block:
                        return;
                    case AbstractNode.Status.Finished:
                    case AbstractNode.Status.Continue:
                        currentNode = currentNode.nextNode;
                        break;
                }
            }
        }

        public VaribleType GetVaribleType(string varName)
        {
            for (int i = 0; i < variablePairs.Length; i++)
            {
                if (variablePairs[i].keyName == varName)
                {
                    return variablePairs[i].keyType;
                }
            }

            return VaribleType.None;
        }

        [System.Serializable]
        public struct VariablePair {
            public string keyName;
            public VaribleType keyType;
        }

        public enum VaribleType
        {
            Int,
            Float,
            Bool,
            None,
        }

#if UNITY_EDITOR
        public void GetVariableNames(out string[] names, out VaribleType[] types)
        {
            names = new string[variablePairs.Length];
            types = new VaribleType[variablePairs.Length];
            for (int i = 0; i < variablePairs.Length; i++)
            {
                names[i] = variablePairs[i].keyName;
                types[i] = variablePairs[i].keyType;
            }
        }

        public void OnCreated()
        {
            if (nodes.Count <= 0)
            {
                Node.graphHotfix = this;
                Node node = ScriptableObject.CreateInstance(typeof(StartNode)) as Node;
                node.name = "Start";
                node.graph = this;
                nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this))) AssetDatabase.AddObjectToAsset(node, this);
                AssetDatabase.SaveAssets();
            }
        }

        [MenuItem("Assets/Create/Dialogue Graph", false, 0)]
        public static void CreateAsset()
        {
            DialogueGraph asset = ScriptableObject.CreateInstance<DialogueGraph>();

            AssetDatabase.CreateAsset(asset, "Assets/NewDialogueGraph.asset");
            AssetDatabase.SaveAssets();

            asset.OnCreated();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
#endif
    }
}