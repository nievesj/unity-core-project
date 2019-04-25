using System;
using System.Collections;
using System.Collections.Generic;
using Core.Services;
using UnityEngine;
using UniRx;

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
        
        private readonly Subject<CoreAnimationEvent> _onStateEnter = new Subject<CoreAnimationEvent>();
        private readonly Subject<CoreAnimationEvent> _onStateExit = new Subject<CoreAnimationEvent>();
        
        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            _animationTriggers = _animator.GetBehaviours<StateMachineAnimationTrigger>();
            foreach (var trigger in _animationTriggers)
            {
                trigger.OnEnterStateEvent().Subscribe(OnEnterStateEvent);
                trigger.OnExitStateEvent().Subscribe(OnExitStateEvent);
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
            _onStateEnter.OnNext(value);
        }

        protected virtual void OnExitStateEvent(CoreAnimationEvent value)
        {
            _onStateExit.OnNext(value);
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
