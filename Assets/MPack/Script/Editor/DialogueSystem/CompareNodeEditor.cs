using XNodeEditor;
using UnityEditor;


namespace MPack
{
    [CustomNodeEditor(typeof(CompareNode))]
    public class CompareNodeEditor : NodeEditor
    {
        private DialogueGraph graph;
        private CompareNode node;

        private SerializedProperty booleanInput1;
        private SerializedProperty booleanInput2;

        private SerializedProperty intInput1;
        private SerializedProperty intInput2;

        private SerializedProperty floatInput1;
        private SerializedProperty floatInput2;

        private SerializedProperty booleanOutput;

        public override void OnBodyGUI()
        {
            if (node == null)
            {
                node = (CompareNode)target;
                graph = (DialogueGraph)node.graph;

                booleanInput1 = serializedObject.FindProperty("BooleanInput1");
                booleanInput2 = serializedObject.FindProperty("BooleanInput2");

                intInput1 = serializedObject.FindProperty("IntInput1");
                intInput2 = serializedObject.FindProperty("IntInput2");

                floatInput1 = serializedObject.FindProperty("FloatInput1");
                floatInput2 = serializedObject.FindProperty("FloatInput2");

                booleanOutput = serializedObject.FindProperty("BooleanOutput");
            }

            EditorGUI.BeginChangeCheck();
            DialogueGraph.VaribleType varibleType = (DialogueGraph.VaribleType)EditorGUILayout.EnumPopup(node.compareValueType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(node, "");
                node.compareValueType = varibleType;
            }

            serializedObject.Update();
            CompareNode.NumberOperatorType numberOperatorType;

            switch (varibleType)
            {
                case DialogueGraph.VaribleType.Bool:
                    EditorGUI.BeginChangeCheck();
                    CompareNode.BooleanOperatorType boolOperatorType = (CompareNode.BooleanOperatorType)EditorGUILayout.EnumPopup(node.booleanOperatorType);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(node, "");
                        node.booleanOperatorType = boolOperatorType;
                    }

                    NodeEditorGUILayout.PropertyField(booleanInput1);
                    NodeEditorGUILayout.PropertyField(booleanInput2);
                    break;
                case DialogueGraph.VaribleType.Int:
                    EditorGUI.BeginChangeCheck();
                    numberOperatorType = (CompareNode.NumberOperatorType)EditorGUILayout.EnumPopup(node.numberOperatorType);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(node, "");
                        node.numberOperatorType = numberOperatorType;
                    }

                    NodeEditorGUILayout.PropertyField(intInput1);
                    NodeEditorGUILayout.PropertyField(intInput2);
                    break;
                case DialogueGraph.VaribleType.Float:
                    EditorGUI.BeginChangeCheck();
                    numberOperatorType = (CompareNode.NumberOperatorType)EditorGUILayout.EnumPopup(node.numberOperatorType);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(node, "");
                        node.numberOperatorType = numberOperatorType;
                    }

                    NodeEditorGUILayout.PropertyField(floatInput1);
                    NodeEditorGUILayout.PropertyField(floatInput2);
                    break;
            }

            NodeEditorGUILayout.PropertyField(booleanOutput);

            serializedObject.ApplyModifiedProperties();
        }
    }
}