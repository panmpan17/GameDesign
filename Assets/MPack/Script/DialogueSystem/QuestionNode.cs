using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace MPack
{
    [NodeWidth(300)]
    public class QuestionNode : AbstractNode
    {
        [Input]
        public NodeEmptyIO Input;

        [TextArea]
        public string content;

        [HideInInspector]
        public Choice[] choices;

        [HideInInspector]
        public int choiceCount;

        [System.Serializable]
        public struct Choice
        {
            [TextArea]
            public string content;
            public NodePort port;

            public int index;

            public override string ToString()
            {
                return string.Format("<Choice '{0}'>", content);
            }
        }

        public override void Proccess()
        {
            if (status == Status.Finished)
            {
                status = Status.Continue;
                return;
            }

            DialogueGraph dialogueGraph = (DialogueGraph)graph;
            dialogueGraph.DialogueInterpreter.ChangeToQuestion(this);

            status = Status.Block;
        }

        public void MakeChoice(int choiceIndex)
        {
            nextNode = (AbstractNode)choices[choiceIndex].port.Connection.node;
            status = Status.Finished;
        }

    #if UNITY_EDITOR
        public void AddNewChoice()
        {
            Choice[] newChoices = new Choice[choices.Length + 1];
            for (int i = 0; i < choices.Length; i++)
                newChoices[i] = choices[i];

            Choice choice = new Choice();
            choice.index = choiceCount++;
            choice.port = AddDynamicOutput(typeof(NodeEmptyIO), ConnectionType.Override, TypeConstraint.Strict, "choice_port_" + choice.index);

            newChoices[newChoices.Length - 1] = choice;
            choices = newChoices;
        }

        public void RemoveChoiceAt(int index)
        {
            Choice[] newChoices = new Choice[choices.Length - 1];

            int loopIndex = 0;
            for (int i = 0; i < choices.Length; i++)
            {
                if (i != index)
                {
                    newChoices[loopIndex++] = choices[i];
                }
            }

            choices = newChoices;
        }
    #endif
    }
}