using Core.Reactive;
using UnityEngine;

namespace Core.Animation
{
    public class StateMachineAnimationTrigger : StateMachineBehaviour
    {
        [SerializeField]
        private CoreAnimationEvent coreAnimationEvent;

        public CoreReactiveProperty<CoreAnimationEvent> OnEnterEvent { get; private set; } = new CoreReactiveProperty<CoreAnimationEvent>();
        public CoreReactiveProperty<CoreAnimationEvent> OnExitEvent { get; private set; } = new CoreReactiveProperty<CoreAnimationEvent>();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnEnterEvent.Value = coreAnimationEvent;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnExitEvent.Value = coreAnimationEvent;
        }
    }
}