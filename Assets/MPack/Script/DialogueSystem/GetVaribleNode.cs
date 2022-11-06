using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace MPack
{
    public class GetVaribleNode : AbstractNode
    {
        public DialogueGraph.VaribleType varibleType;

        [Output]
        public bool BooleanOutput;
        [Output]
        public float FloatOutput;
        [Output]
        public int IntOutput;

        [VaribleName]
        public string variableName;


        public override void Proccess()
        {
            throw new System.NotImplementedException();
        }

        public override void PrepareValue()
        {
            DialogueGraph dialogueGraph = (DialogueGraph) graph;
            DialogueGraph.VaribleType varibleType = dialogueGraph.GetVaribleType(variableName);

            switch (varibleType)
            {
                case DialogueGraph.VaribleType.Float:
                    if (dialogueGraph.varibles.GetFloat(variableName, out float floatValue)) FloatOutput = floatValue;
                    else FloatOutput = 0;
                    break;
                case DialogueGraph.VaribleType.Int:
                    if (dialogueGraph.varibles.GetInt(variableName, out int intValue)) IntOutput = intValue;
                    else IntOutput = 0;
                    break;
                case DialogueGraph.VaribleType.Bool:
                    if (dialogueGraph.varibles.GetBool(variableName, out bool booleanValue)) BooleanOutput = booleanValue;
                    else BooleanOutput = false;
                    break;
            }
            // GetValueFromInput();

            // bool result = false;
        }

        public override object GetValue(XNode.NodePort port)
        {
            switch (port.fieldName)
            {
                case "BooleanOutput":
                    return BooleanOutput;
                case "FloatOutput":
                    return FloatOutput;
                case "IntOutput":
                    return IntOutput;
            }
            return null;
        }
    }
}