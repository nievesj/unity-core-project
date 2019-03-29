using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.AI
{
    public interface IAction
    {
        void Enter(IStateMachineData data);
        void Perform(IStateMachineData data);
        void Exit();
        IObservable<IState> OnActionCompleted();
    }
    
    [System.Serializable]
    public struct Transition
    {
        public Decision decision;
        public State successState;
        public State failedState;
        public bool selectRandomStateFromList;
        public State[] ramdomSelection;
    }
    
    public abstract class Action : ScriptableObject, IAction
    {
        [SerializeField]
        protected Transition[] transitions;

        protected Subject<IState> _onActionCompleted;
        
        public abstract void Perform(IStateMachineData data);
        protected abstract void Complete(IStateMachineData data);
        public abstract void Exit();

        public virtual void Enter(IStateMachineData data)
        {
            _onActionCompleted = new Subject<IState>();
            if (transitions.Length > 0)
            {
                for (var i = 0; i < transitions.Length; i++)
                {
                    if (transitions[i].decision)
                        transitions[i].decision.EnterDecision(data);
                }
            }
        }

        //Note: This will exit when the first decision is made, for now it may be ok
        //but it leaves out complex behaviours
        protected virtual void MakeDecision(IStateMachineData data)
        {
            if (transitions.Length > 0)
            {
                for (var i = 0; i < transitions.Length; i++)
                {
                    if (transitions[i].selectRandomStateFromList)
                    {
                        Decided(transitions[i].ramdomSelection.GetRandomElement());
                    }
                    else
                    {
                        var decisionSucceeded = transitions[i].decision.Decide(data);
                        if (decisionSucceeded)
                            Decided(transitions[i].successState);
                        else
                            Decided(transitions[i].failedState);
                    }
                }
            }
        }

        protected virtual void Decided(IState state)
        {
            _onActionCompleted.OnNext(state);
            _onActionCompleted.OnCompleted();
            Exit();
        }

        public virtual IObservable<IState> OnActionCompleted()
        {
            return _onActionCompleted;
        }
    }
}
