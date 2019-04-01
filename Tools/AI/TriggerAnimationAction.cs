using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Animation;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

namespace Core.AI
{
    public enum AnimationActionOption
    {
        Both,
        OnEnter,
        OnExit
    }
    
    public class TriggerAnimationActionBlueprint : ActionBlueprint
    {
        [SerializeField]
        private string animationTrigger;

        [SerializeField]
        private AnimationActionOption animationActionOption;

        [SerializeField]
        private CoreAnimationEvent animationEventEnter;

        [SerializeField]
        private CoreAnimationEvent animationEventExit;
        
        public override IEntityData GetInputValue()
        {
            return null;
        }
        
        public override IEntityData GetOutputValue()
        {
            return null;
        }

        public override Node CreateInstance(NodeBlueprint node)
        {
            //pass on all arguments on the blueprint
            return new TriggerAnimationAction();
        }
    }

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
        
        protected override void StartAction()
        {
            // _subscriptions = new List<IDisposable>
            // {
            //     input.AnimationController.OnEnterStateEvent().Subscribe(OnEnterStateEvent),
            //     input.AnimationController.OnExitStateEvent().Subscribe(OnExitStateEvent),
            // };
            //
            // input.AnimationController.TriggerAnimation(animationTrigger);
        }

        private void Complete()
        {
            // actionState = ActionState.Completed;
            _subscriptions.ForEach(x => x.Dispose());
        }

        private void OnEnterStateEvent(CoreAnimationEvent value)
        {
            if (animationEventEnter && value.name == animationEventEnter.name && animationActionOption == AnimationActionOption.OnEnter)
                Complete();
        }

        private void OnExitStateEvent(CoreAnimationEvent value )
        {
            if (animationEventExit && value.name == animationEventExit.name && (animationActionOption == AnimationActionOption.OnExit || animationActionOption == AnimationActionOption.Both))
                Complete();
        }
    }
}
