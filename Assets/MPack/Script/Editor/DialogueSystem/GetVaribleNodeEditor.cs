using XNodeEditor;
using UnityEditor;


namespace MPack
{
    [CustomNodeEditor(typeof(GetVaribleNode))]
    public class GetVaribleNodeEditor : NodeEditor
    {
        private DialogueGraph graph;
        private GetVaribleNode node;

        public override void OnBodyGUI()
        {
            if (node == null)
            {
                node = (GetVaribleNode)target;
                graph = (DialogueGraph)node.graph;
            }

            serializedObject.Update();

            SerializedProperty property = serializedObject.FindProperty("variableName");
            EditorGUILayout.PropertyField(property);

            serializedObject.ApplyModifiedProperties();

            switch (graph.GetVaribleType(property.stringValue))
            {
                case DialogueGraph.VaribleType.Bool:
                    NodeEditorGUILayout.PortField(node.GetOutputPort("BooleanOutput"));
                    break;
                case DialogueGraph.VaribleType.Int:
                    NodeEditorGUILayout.PortField(node.GetOutputPort("IntOutput"));
                    break;
                case DialogueGraph.VaribleType.Float:
                    NodeEditorGUILayout.PortField(node.GetOutputPort("FloatOutput"));
                    break;
            }
        }
    }
}