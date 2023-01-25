using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MPack
{
    public class VariableReact : MonoBehaviour
    {
        [SerializeField]
        private VariableStorage variableStorage;
        [SerializeField]
        private EventReference saveDataRestoreEvent;
        [SerializeField]
        private VariableMatchPattern variableMatchPattern;


        public UnityEvent OnValueMatched;

        void OnEnable()
        {
            saveDataRestoreEvent.InvokeEvents += CheckVariable;
        }

        void OnDisable()
        {
            saveDataRestoreEvent.InvokeEvents += CheckVariable;
        }

        void CheckVariable()
        {
            if (variableMatchPattern.IsFullyMatched(variableStorage))
                OnValueMatched.Invoke();
        }
    }
}