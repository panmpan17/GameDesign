using XNodeEditor;
using UnityEditor;


namespace MPack
{
    [CustomNodeEditor(typeof(SetVariableNode))]
    public class SetVariableNodeEditor : NodeEditor
    {
        private DialogueGraph graph;
        private SetVariableNode node;

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (node == null)
            {
                node = (SetVariableNode)target;
                graph = (DialogueGraph)node.graph;
            }


            serializedObject.Update();

            SerializedProperty property = serializedObject.FindProperty("variableName");
            EditorGUILayout.PropertyField(property);

            switch (graph.GetVaribleType(property.stringValue))
            {
                case DialogueGraph.VaribleType.Bool:
                    node.variableBoolValue = EditorGUILayout.Toggle("Value", node.variableBoolValue);
                    break;
                case DialogueGraph.VaribleType.Int:
                    node.variableIntValue = EditorGUILayout.IntField("Value", node.variableIntValue);
                    break;
                case DialogueGraph.VaribleType.Float:
                    node.variableFloatValue = EditorGUILayout.FloatField("Value", node.variableFloatValue);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}