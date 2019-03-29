using System;
using System.Collections.Generic;
using Core.Animation;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.AI
{
    public interface IState
    {
        void EnterState(IStateMachineData data);
        void Execute(IStateMachineData data);
        void ExitState();

        IObservable<IState> OnExitState();
    }

    // [CreateAssetMenu(menuName = "AI/State")]
    public class State : ScriptableObject, IState
    {
        [SerializeField]
        protected Action[] actions;

        protected Subject<IState> _onExitState;
        protected List<IDisposable> subscriptions;
        
        public virtual void EnterState(IStateMachineData data)
        {
            subscriptions = new List<IDisposable>();
            _onExitState = new Subject<IState>();
            
            if (actions.Length > 0)
            {
                for (var i = 0; i < actions.Length; i++)
                {
                    actions[i].Enter(data);
                    subscriptions.Add(actions[i].OnActionCompleted().Subscribe(OnActionCompleted));
                }
            }
        }

        public virtual void Execute(IStateMachineData data)
        {
            PerformActions(data);
        }

        public void ExitState()
        {
            subscriptions.ForEach(x => x.Dispose());
            actions.ForEach(a => a.Exit());
        }

        protected virtual void PerformActions(IStateMachineData data)
        {
            if (actions.Length > 0)
            {
                for (var i = 0; i < actions.Length; i++)
                    actions[i].Perform(data);
            }
        }

        protected virtual void OnActionCompleted(IState state)
        {
            ExitState();
            
            _onExitState.OnNext(state);
            _onExitState.OnCompleted();
        }

        public IObservable<IState> OnExitState()
        {
            return _onExitState;
        }
    }
}