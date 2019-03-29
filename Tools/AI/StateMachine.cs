using System;
using Core.Animation;
using Core.Services;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Core.AI
{
    public interface IStateMachineData
    {
        AnimationController AnimationController { get;}
        NavMeshAgent NavMeshAgent { get;}
        Transform Target { get; }
    }

    public interface IStateMachine
    {
        bool IsStateMachineActive { get; }
        IState DefaultState { get; }
        IState CurrentState { get; }
        IStateMachineData StateMachineData { get; }
        IDisposable Subscription { get; }

        void EnterState(IState state);
        void ExitState(IState state);
        void Execute();
    }

    public abstract class StateMachine : CoreBehaviour, IStateMachine
    {
        [SerializeField]
        private IState defaultState;

        public IState DefaultState => defaultState;
        
        public bool IsStateMachineActive { get; protected set; }
        public IState CurrentState { get; protected set; }
        public IStateMachineData StateMachineData { get; protected set; }
        public IDisposable Subscription { get; protected set; }

        protected virtual void Start()
        {
            if (DefaultState != null)
                EnterState(DefaultState);
        }

        public virtual void EnterState(IState state)
        {
            state.EnterState(StateMachineData);
            Subscription = state.OnExitState().Subscribe(OnExitCurrentState);
        }

        public virtual void ExitState(IState state)
        {
            CurrentState = null;
        }

        protected virtual void OnExitCurrentState(IState nextState)
        {
            ExitState(CurrentState);
            EnterState(nextState);
        }

        public virtual void Execute()
        {
            CurrentState?.Execute(StateMachineData);
        }
    }
}