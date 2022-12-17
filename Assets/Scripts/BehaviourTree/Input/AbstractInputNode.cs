using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace XnodeBehaviourTree
{
    public abstract class AbstractInputNode : Node
    {
        public abstract void ProcessData();
    }
}