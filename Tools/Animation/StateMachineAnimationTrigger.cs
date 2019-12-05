using Core.Reactive;
using UnityEngine;

namespace Core.Animation
{
    public class StateMachineAnimationTrigger : StateMachineBehaviour
    {
        [SerializeField]
        private CoreAnimationEvent coreAnimationEvent;

        public CoreEvent<CoreAnimationEvent> OnEnterEvent { get; private set; } = new CoreEvent<CoreAnimationEvent>();
        public CoreEvent<CoreAnimationEvent> OnExitEvent { get; private set; } = new CoreEvent<CoreAnimationEvent>();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnEnterEvent.Broadcast(coreAnimationEvent);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnExitEvent.Broadcast(coreAnimationEvent);
        }
    }
}