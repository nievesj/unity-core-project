using System;
using UniRx;
using UnityEngine;

namespace Core.Tools.Animation
{
    public class StateMachineAnimationTrigger : StateMachineBehaviour
    {
        [SerializeField]
        private CoreAnimationEvent coreAnimationEvent;

        private readonly Subject<CoreAnimationEvent> _onStateEnter = new Subject<CoreAnimationEvent>();
        private readonly Subject<CoreAnimationEvent> _onStateExit = new Subject<CoreAnimationEvent>();
        public IObservable<CoreAnimationEvent> OnEnterEvent => _onStateEnter;
        public IObservable<CoreAnimationEvent> OnExitEvent => _onStateExit;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _onStateEnter.OnNext(coreAnimationEvent);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _onStateExit.OnNext(coreAnimationEvent);
        }

        private void OnDestroy()
        {
            _onStateEnter.OnCompleted();
            _onStateExit.OnCompleted();
        }
    }
}