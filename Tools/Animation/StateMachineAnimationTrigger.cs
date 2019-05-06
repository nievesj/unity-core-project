using UniRx;
using UnityEngine;

namespace Core.Animation
{
    public class StateMachineAnimationTrigger : StateMachineBehaviour
    {
        [SerializeField]
        private CoreAnimationEvent coreAnimationEvent;

        public ReactiveProperty<CoreAnimationEvent> OnEnterEvent { get; private set; } = new ReactiveProperty<CoreAnimationEvent>();
        public ReactiveProperty<CoreAnimationEvent> OnExitEvent { get; private set; } = new ReactiveProperty<CoreAnimationEvent>();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnEnterEvent.SetValueAndForceNotify(coreAnimationEvent);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnExitEvent.SetValueAndForceNotify(coreAnimationEvent);
        }
    }
}