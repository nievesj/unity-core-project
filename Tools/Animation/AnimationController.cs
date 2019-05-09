using System.Collections.Generic;
using Core.Reactive;
using Core.Services;
using UniRx;
using UnityEngine;

namespace Core.Animation
{
    public abstract class AnimationController : CoreBehaviour
    {
        [SerializeField]
        protected List<string> deathTriggers;

        [SerializeField]
        protected List<string> attacktriggers;

        [SerializeField]
        protected List<string> damageTriggers;

        protected Animator _animator;
        protected float _movementSpeed;
        protected StateMachineAnimationTrigger[] _animationTriggers;

        public CoreReactiveProperty<CoreAnimationEvent> OnEnterEvent { get; private set; } = new CoreReactiveProperty<CoreAnimationEvent>();
        public CoreReactiveProperty<CoreAnimationEvent> OnExitEvent { get; private set; } = new CoreReactiveProperty<CoreAnimationEvent>();

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            _animationTriggers = _animator.GetBehaviours<StateMachineAnimationTrigger>();
            foreach (var trigger in _animationTriggers)
            {
                trigger.OnEnterEvent.Subscribe(OnEnterStateEvent);
                trigger.OnExitEvent.Subscribe(OnExitStateEvent);
            }
        }

        public virtual void SetMovementSpeed(float value)
        {
            _movementSpeed = value;
            _animator.SetFloat("MovementSpeed", _movementSpeed);
        }

        public virtual void TriggerDeath()
        {
            _animator.SetTrigger(deathTriggers.GetRandomElement());
        }

        public virtual void TakeDamage()
        {
            _animator.SetTrigger(damageTriggers.GetRandomElement());
        }

        public virtual void Attack()
        {
            _animator.SetTrigger(attacktriggers.GetRandomElement());
        }

        public virtual void Attack(bool isLoop)
        {
            _animator.SetBool("IsAttackLoop", isLoop);
        }

        public virtual void TriggerAnimation(string trigger)
        {
            _animator.SetTrigger(trigger);
        }

        protected virtual void OnEnterStateEvent(CoreAnimationEvent value)
        {
            OnEnterEvent.Value = value;
        }

        protected virtual void OnExitStateEvent(CoreAnimationEvent value)
        {
            OnExitEvent.Value = value;
        }

        // public IObservable<CoreAnimationEvent> OnEnterStateEvent()
        // {
        //     return _onStateEnter;
        // }
        //
        // public IObservable<CoreAnimationEvent> OnExitStateEvent()
        // {
        //     return _onStateExit;
        // }
    }
}