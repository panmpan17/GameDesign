using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack
{
    public interface IDialogueInterpreter
    {
        void ChangeToQuestion(QuestionNode node);
        void ChangeToDialogue(DialogueNode node);
        void OnDialogueEnd();
    }
}