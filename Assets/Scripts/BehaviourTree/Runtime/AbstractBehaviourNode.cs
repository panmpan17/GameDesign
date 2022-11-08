using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace XnodeBehaviourTree
{
    public abstract class AbstractBehaviourNode : Node
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        [System.NonSerialized] public State state = State.Running;
        [System.NonSerialized] public bool started = false;
        // [HideInInspector] public Context context;
        // [HideInInspector] public Blackboard blackboard;

        public State Update()
        {
            if (!started)
            {
                OnStart();
                started = true;
            }

            state = OnUpdate();

            if (state != State.Running)
            {
                OnStop();
                started = false;
            }

            return state;
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        public void Abort()
        {
            // TODO: abort
        }

        public virtual void OnDrawGizmos() { }
        public virtual void DrawGizmos(Transform transform) { }

        public abstract void OnInitial();
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
}