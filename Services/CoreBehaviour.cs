using System;
using Core.Services.Assets;
using Core.Services.Audio;
using Core.Services.Factory;
using Core.Services.UI;
using UnityEngine;
using Zenject;
using UniRx;

namespace Core.Services
{
    public abstract class Service : IInitializable, IDisposable
    {
        public virtual void Initialize() { }
        public virtual void Dispose() { }
    }

    public abstract class CoreBehaviour : MonoBehaviour
    {
        [Inject]
        protected UIService UiService;
        
        [Inject]
        protected AudioService AudioService;
        
        [Inject]
        protected AssetService AssetService;
        
        [Inject]
        protected FactoryService FactoryService;
        
        private readonly Subject<CoreBehaviour> _onDestroyed = new Subject<CoreBehaviour>();

        protected virtual void Awake(){ }

        protected virtual void Start()
        {
            UiService.OnGamePaused().Subscribe(OnGamePaused);
        }

        protected virtual void OnGamePaused(bool isPaused){}
        
        /// <summary>
        /// Triggered when the object is destroyed
        /// </summary>
        /// <returns></returns>
        public IObservable<CoreBehaviour> OnDestroyed()
        {
            return _onDestroyed;
        }

        protected virtual void OnDestroy()
        {
            _onDestroyed.OnNext(this);
            _onDestroyed.OnCompleted();
        }
    }
}