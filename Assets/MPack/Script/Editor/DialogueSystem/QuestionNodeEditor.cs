using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;


namespace MPack
{
    [CustomNodeEditor(typeof(QuestionNode))]
    public class QuestionNodeEditor : NodeEditor
    {
        private DialogueGraph graph;
        private QuestionNode node;

        private SerializedProperty input;
        private SerializedProperty speaker;
        private SerializedProperty contentLanguageID;
        private SerializedProperty choices;

        public override void OnBodyGUI()
        {
            if (node == null)
            {
                node = (QuestionNode)target;
                graph = (DialogueGraph)node.graph;

                if (node.choices == null) node.choices = new QuestionNode.Choice[0];

                for (int i = 0; i < node.choices.Length; i++)
                {
                    node.choices[i].port = node.GetDynamicOutput("choice_port_" + node.choices[i].index);
                }

                input = serializedObject.FindProperty("Input");
                speaker = serializedObject.FindProperty("Speaker");
                contentLanguageID = serializedObject.FindProperty("ContentLaguageID");
                choices = serializedObject.FindProperty("choices");
            }

            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(input);
            NodeEditorGUILayout.PropertyField(speaker);
            NodeEditorGUILayout.PropertyField(contentLanguageID);

            EditorGUILayout.Space();

            for (int i = 0; i < choices.arraySize; i++)
            {
                SerializedProperty choiceProperty = choices.GetArrayElementAtIndex(i);
                NodeEditorGUILayout.PropertyField(choiceProperty);

                SerializedProperty portProperty = choiceProperty.FindPropertyRelative("port");
                NodePort port = node.choices[i].port;

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    Undo.RecordObject(node, "");
                    node.RemoveDynamicPort(port);
                    node.RemoveChoiceAt(i);
                    return;
                }

                NodeEditorGUILayout.PortField(port);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
            }

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("+"))
            {
                Undo.RecordObject(node, "");
                node.AddNewChoice();
            }
        }
    }

    [CustomPropertyDrawer(typeof(QuestionNode.Choice))]
    public class ChoiceProperty : PropertyDrawer
    {
        SerializedProperty contentLanguageID;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            contentLanguageID = property.FindPropertyRelative("ContentLaguageID");
            return EditorGUI.GetPropertyHeight(contentLanguageID);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, contentLanguageID, new GUIContent("ID"));
        }
    }
}