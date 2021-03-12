using System;
using System.Collections.Generic;
using Core.Common.Extensions.IEnumerable;
using Core.Services;
using UniRx;
using UnityEngine;

namespace Core.Tools.Animation
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

        private readonly Subject<CoreAnimationEvent> _onStateEnter = new Subject<CoreAnimationEvent>();
        private readonly Subject<CoreAnimationEvent> _onStateExit = new Subject<CoreAnimationEvent>();

        public IObservable<CoreAnimationEvent> OnEnterEvent => _onStateEnter;
        public IObservable<CoreAnimationEvent> OnExitEvent => _onStateExit;

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
            _onStateEnter.OnCompleted();
            _onStateExit.OnCompleted();
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
            _onStateEnter.OnNext(value);
        }

        protected virtual void OnExitStateEvent(CoreAnimationEvent value)
        {
            _onStateExit.OnNext(value);
        }
    }
}