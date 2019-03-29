using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Animation;
using UniRx;
using UnityEngine;

namespace Core.AI
{
    public enum AnimationActionOption
    {
        Both,
        OnEnter,
        OnExit
    }

    [CreateAssetMenu(menuName = "AI/Actions/TriggerAnimationAction")]
    public class TriggerAnimationAction : Action
    {
        [SerializeField]
        private string animationTrigger;

        [SerializeField]
        private AnimationActionOption animationActionOption;

        [SerializeField]
        private CoreAnimationEvent animationEventEnter;

        [SerializeField]
        private CoreAnimationEvent animationEventExit;

        private List<IDisposable> _subscriptions;

        public override void Enter(IStateMachineData data)
        {
            base.Enter(data);
            _subscriptions = new List<IDisposable>
            {
                data.AnimationController.OnEnterStateEvent().Subscribe(x => OnEnterStateEvent(x, data)),
                data.AnimationController.OnExitStateEvent().Subscribe(x => OnExitStateEvent(x, data)),
            };

            data.AnimationController.TriggerAnimation(animationTrigger);
        }

        public override void Perform(IStateMachineData data) { }

        public override void Exit()
        {
            _subscriptions.ForEach(x => x.Dispose());
        }

        protected override void Complete(IStateMachineData data)
        {
            MakeDecision(data);
        }

        private void OnEnterStateEvent(CoreAnimationEvent value, IStateMachineData data)
        {
            if (animationEventEnter && value.name == animationEventEnter.name && animationActionOption == AnimationActionOption.OnEnter)
                Complete(data);
        }

        private void OnExitStateEvent(CoreAnimationEvent value, IStateMachineData data)
        {
            if (animationEventExit && value.name == animationEventExit.name && (animationActionOption == AnimationActionOption.OnExit || animationActionOption == AnimationActionOption.Both))
                Complete(data);
        }
    }
}