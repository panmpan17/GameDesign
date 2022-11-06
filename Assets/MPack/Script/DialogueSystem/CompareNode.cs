using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace MPack
{
    public class CompareNode : AbstractNode
    {
        public DialogueGraph.VaribleType compareValueType;

        [Input(typeConstraint: TypeConstraint.Strict)]
        public float FloatInput1;
        [Input(typeConstraint: TypeConstraint.Strict)]
        public float FloatInput2;

        [Input(typeConstraint: TypeConstraint.Strict)]
        public int IntInput1;
        [Input(typeConstraint: TypeConstraint.Strict)]
        public int IntInput2;

        [HideInInspector]
        public NumberOperatorType numberOperatorType;

        [Input(typeConstraint: TypeConstraint.Strict)]
        public bool BooleanInput1;
        [Input(typeConstraint: TypeConstraint.Strict)]
        public bool BooleanInput2;

        [HideInInspector]
        public BooleanOperatorType booleanOperatorType;

        [Output(typeConstraint: TypeConstraint.Strict)]
        public bool BooleanOutput;

        public enum BooleanOperatorType {
            Equal,
            NotEqual,
        }

        public enum NumberOperatorType
        {
            Equal,
            NotEqual,
            Bigger,
            Smaller,
            BiggerAndEqual,
            SmallerAndEqual,
        }

        public override void Proccess()
        {
            throw new System.NotImplementedException();
        }

        public override void PrepareValue()
        {
            GetValueFromInput();

            bool result = false;

            switch (compareValueType)
            {
                case DialogueGraph.VaribleType.Float:
                    switch (numberOperatorType)
                    {
                        case NumberOperatorType.Equal:
                            result = FloatInput1 == FloatInput2;
                            break;
                        case NumberOperatorType.NotEqual:
                            result = FloatInput1 != FloatInput2;
                            break;
                        case NumberOperatorType.Bigger:
                            result = FloatInput1 > FloatInput2;
                            break;
                        case NumberOperatorType.Smaller:
                            result = FloatInput1 < FloatInput2;
                            break;
                        case NumberOperatorType.BiggerAndEqual:
                            result = FloatInput1 >= FloatInput2;
                            break;
                        case NumberOperatorType.SmallerAndEqual:
                            result = FloatInput1 <= FloatInput2;
                            break;
                    }
                    break;
                case DialogueGraph.VaribleType.Int:
                    switch (numberOperatorType)
                    {
                        case NumberOperatorType.Equal:
                            result = IntInput1 == IntInput2;
                            break;
                        case NumberOperatorType.NotEqual:
                            result = IntInput1 != IntInput2;
                            break;
                        case NumberOperatorType.Bigger:
                            result = IntInput1 > IntInput2;
                            break;
                        case NumberOperatorType.Smaller:
                            result = IntInput1 < IntInput2;
                            break;
                        case NumberOperatorType.BiggerAndEqual:
                            result = IntInput1 >= IntInput2;
                            break;
                        case NumberOperatorType.SmallerAndEqual:
                            result = IntInput1 <= IntInput2;
                            break;
                    }
                    break;
                case DialogueGraph.VaribleType.Bool:
                    switch (booleanOperatorType)
                    {
                        case BooleanOperatorType.Equal:
                            result = BooleanInput1 == BooleanInput2;
                            break;
                        case BooleanOperatorType.NotEqual:
                            result = BooleanInput1 != BooleanInput2;
                            break;
                    }
                    break;
            }

            BooleanOutput = result;
        }

        private void GetValueFromInput()
        {
            NodePort nodePort;
            AbstractNode node;

            switch (compareValueType)
            {
                case DialogueGraph.VaribleType.Int:
                    nodePort = GetInputPort("IntInput1");

                    if (nodePort.IsConnected)
                    {
                        node = (AbstractNode)nodePort.Connection.node;
                        node.PrepareValue();
                        IntInput1 = nodePort.GetInputValue<int>();
                    }

                    nodePort = GetInputPort("IntInput2");

                    if (nodePort.IsConnected)
                    {
                        node = (AbstractNode)nodePort.Connection.node;
                        node.PrepareValue();
                        IntInput2 = nodePort.Connection.GetInputValue<int>();
                    }
                    break;
                case DialogueGraph.VaribleType.Float:
                    nodePort = GetInputPort("FloatInput1");

                    if (nodePort.IsConnected)
                    {
                        node = (AbstractNode)nodePort.Connection.node;
                        node.PrepareValue();
                        FloatInput1 = nodePort.Connection.GetInputValue<float>();
                    }

                    nodePort = GetInputPort("FloatInput2");

                    if (nodePort.IsConnected)
                    {
                        node = (AbstractNode)nodePort.Connection.node;
                        node.PrepareValue();
                        FloatInput2 = nodePort.Connection.GetInputValue<float>();
                    }
                    break;
                case DialogueGraph.VaribleType.Bool:
                    nodePort = GetInputPort("BooleanInput1");

                    if (nodePort.IsConnected)
                    {
                        node = (AbstractNode)nodePort.Connection.node;
                        node.PrepareValue();
                        BooleanInput1 = nodePort.Connection.GetInputValue<bool>();
                    }

                    nodePort = GetInputPort("BooleanInput2");

                    if (nodePort.IsConnected)
                    {
                        node = (AbstractNode)nodePort.Connection.node;
                        node.PrepareValue();
                        BooleanInput2 = nodePort.Connection.GetInputValue<bool>();
                    }
                    break;
            }
        }

        public override object GetValue(XNode.NodePort port)
        {
            return BooleanOutput;
        }
    }
}