using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Animation
{
    public class StateMachineAnimationTrigger : StateMachineBehaviour
    {
        [FormerlySerializedAs("animationEvent")]
        [SerializeField]
        private CoreAnimationEvent coreAnimationEvent;
        
        private readonly Subject<CoreAnimationEvent> _onStateEnter = new Subject<CoreAnimationEvent>();
        private readonly Subject<CoreAnimationEvent> _onStateExit = new Subject<CoreAnimationEvent>();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _onStateEnter.OnNext(coreAnimationEvent);
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _onStateExit.OnNext(coreAnimationEvent);
        }
        
        public IObservable<CoreAnimationEvent> OnEnterStateEvent()
        {
            return _onStateEnter;
        }
        
        public IObservable<CoreAnimationEvent> OnExitStateEvent()
        {
            return _onStateExit;
        }
    }
}
