using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Composite/Paralle")]
    public class ParallelNode : AbstractCompositeNode
    {
        State[] childrenLeftToExecute;

        public override void OnInitial()
        {
            base.OnInitial();

            childrenLeftToExecute = new State[children.Length];
        }

        protected override void OnStart()
        {
            for (int i = 0; i < childrenLeftToExecute.Length; i++)
            {
                childrenLeftToExecute[i] = State.Running;
            }
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            bool stillRunning = false;
            for (int i = 0; i < childrenLeftToExecute.Length; ++i)
            {
                if (childrenLeftToExecute[i] == State.Running)
                {
                    var status = children[i].Update();
                    if (status == State.Failure)
                    {
                        AbortRunningChildren();
                        return State.Failure;
                    }

                    if (status == State.Running)
                    {
                        stillRunning = true;
                    }

                    childrenLeftToExecute[i] = status;
                }
            }

            return stillRunning ? State.Running : State.Success;
        }

        void AbortRunningChildren()
        {
            for (int i = 0; i < childrenLeftToExecute.Length; ++i)
            {
                if (childrenLeftToExecute[i] == State.Running)
                {
                    children[i].Abort();
                }
            }
        }
    }
}