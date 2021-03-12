using System;
using Core.Common.Extensions.UnityComponent;
using UniRx;
using UnityEngine;

namespace Core.Systems
{
    public interface ICoreSystem
    {
        void Initialize();
        void Dispose();
        IObservable<Unit> OnCoreSystemInitialized();
    }

    public abstract class SystemBehaviour : MonoBehaviour, ICoreSystem
    {
        //After all systems are initialized, call OnGameStarted, and this can be an injectable event, same with OnGamePaused
        //OnGameFocusChange, etc
        protected readonly Subject<Unit> _onCoreSystemInitialized = new Subject<Unit>();

        protected virtual void Awake()
        {
            Initialize();
        }

        public virtual IObservable<Unit> OnCoreSystemInitialized()
        {
            return _onCoreSystemInitialized;
        }

        public abstract void Initialize();
        public abstract void Dispose();
    }

    public abstract class CoreSystem : SystemBehaviour
    {
        public override void Initialize()
        {
            _onCoreSystemInitialized.OnNext(Unit.Default);
            _onCoreSystemInitialized.OnCompleted();

            DontDestroyOnLoad(gameObject);
        }

        public override void Dispose()
        {
            gameObject.Destroy();
        }
    }
}