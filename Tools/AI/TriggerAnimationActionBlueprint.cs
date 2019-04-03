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

    [System.Serializable]
    public struct AnimationActionData
    {
        public string animationTrigger;

        public AnimationActionOption animationActionOption;

        public CoreAnimationEvent animationEventEnter;

        public CoreAnimationEvent animationEventExit;
    }
    
    public class TriggerAnimationActionBlueprint : ActionBlueprint
    {
        [SerializeField]
        private AnimationActionData animationActionData;

        public override Node CreateNodeInstance(IEntityData data)
        {
            //pass on all arguments on the blueprint
            return new TriggerAnimationAction(data, animationActionData);
        }
    }

    public class TriggerAnimationAction : Action
    {
        private AnimationActionData _animationActionData;

        private IEntityData _data;

        private List<IDisposable> _subscriptions;

        public TriggerAnimationAction(IEntityData data, AnimationActionData animationData)
        {
            _animationActionData = animationData;
            _data = data;
        }
       
        protected override void StartAction()
        {
            _subscriptions = new List<IDisposable>
            {
                _data.AnimationController.OnEnterStateEvent().Subscribe(OnEnterStateEvent),
                _data.AnimationController.OnExitStateEvent().Subscribe(OnExitStateEvent),
            };
            
            _data.AnimationController.TriggerAnimation(_animationActionData.animationTrigger);
        }

        private void Complete()
        {
            ActionState = ActionState.Completed;
            _subscriptions.ForEach(x => x.Dispose());
        }

        private void OnEnterStateEvent(CoreAnimationEvent value)
        {
            if (_animationActionData.animationEventEnter && value.name == _animationActionData.animationEventEnter.name && _animationActionData.animationActionOption == AnimationActionOption.OnEnter)
                Complete();
        }

        private void OnExitStateEvent(CoreAnimationEvent value )
        {
            if (_animationActionData.animationEventExit && value.name == _animationActionData.animationEventExit.name && (_animationActionData.animationActionOption == AnimationActionOption.OnExit || _animationActionData.animationActionOption == AnimationActionOption.Both))
                Complete();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
