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
        protected CompositeDisposable _subscriptions;

        public CoreEvent<CoreAnimationEvent> OnEnterEvent { get; private set; } = new CoreEvent<CoreAnimationEvent>();
        public CoreEvent<CoreAnimationEvent> OnExitEvent { get; private set; } = new CoreEvent<CoreAnimationEvent>();

        protected virtual void Awake()
        {
            _subscriptions = new CompositeDisposable();
            _animator = GetComponent<Animator>();
            _animationTriggers = _animator.GetBehaviours<StateMachineAnimationTrigger>();
        }

        public virtual void Init()
        {
            _animationTriggers = _animator.GetBehaviours<StateMachineAnimationTrigger>();
            foreach (var trigger in _animationTriggers)
            {
                trigger.OnEnterEvent
                    .Subscribe(OnEnterStateEvent)
                    .AddTo(_subscriptions);

                trigger.OnExitEvent
                    .Subscribe(OnExitStateEvent)
                    .AddTo(_subscriptions);
            }
        }

        public virtual void DeInit()
        {
            _subscriptions.Clear();
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
            OnEnterEvent.Broadcast(value);
        }

        protected virtual void OnExitStateEvent(CoreAnimationEvent value)
        {
            OnExitEvent.Broadcast(value);
        }
    }
}