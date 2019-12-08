using UniRx;
using UnityEngine;

namespace Core.Animation
{
    public class StateMachineAnimationTrigger : StateMachineBehaviour
    {
        [SerializeField]
        private CoreAnimationEvent coreAnimationEvent;

        public RxEvent<CoreAnimationEvent> OnEnterEvent { get; private set; } = new RxEvent<CoreAnimationEvent>();
        public RxEvent<CoreAnimationEvent> OnExitEvent { get; private set; } = new RxEvent<CoreAnimationEvent>();

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