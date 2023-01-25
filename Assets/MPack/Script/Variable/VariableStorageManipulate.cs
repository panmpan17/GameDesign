using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace MPack
{
    public class VariableStorageManipulate : MonoBehaviour
    {
        [SerializeField]
        private string variableName;
        [SerializeField]
        private bool boolValue;

        [SerializeField]
        private VariableStorage variableStorage;

        public void Trigger()
        {
            variableStorage.Set(variableName, boolValue);
        }
    }
}
