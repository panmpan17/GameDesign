using UnityEditor;
using UnityEngine;
using XNodeEditor;


namespace MPack
{
    [CustomNodeEditor(typeof(QuestionNode))]
    public class QuestionNodeEditor : NodeEditor
    {
        private DialogueGraph graph;
        private QuestionNode node;

        public override void OnBodyGUI()
        {
            // base.OnBodyGUI();
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("content"));

            if (node == null)
            {
                node = (QuestionNode)target;
                graph = (DialogueGraph)node.graph;

                if (node.choices == null) node.choices = new QuestionNode.Choice[0];

                for (int i = 0; i < node.choices.Length; i++)
                {
                    node.choices[i].port = node.GetDynamicOutput("choice_port_" + node.choices[i].index);
                }
            }


            for (int i = 0; i < node.choices.Length; i++)
            {
                QuestionNode.Choice choice = node.choices[i];

                EditorGUI.BeginChangeCheck();
                node.choices[i].content = EditorGUILayout.TextArea(choice.content);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(node, "");
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    Undo.RecordObject(node, "");
                    node.RemoveDynamicPort(choice.port);
                    node.RemoveChoiceAt(i);
                    return;
                }

                NodeEditorGUILayout.PortField(choice.port);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
            }

            if (GUILayout.Button("+"))
            {
                Undo.RecordObject(node, "");
                node.AddNewChoice();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}