using System;
using Core.Services.Assets;
using Core.Services.Audio;
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

        protected virtual void Awake(){ }

        protected virtual void Start()
        {
            UiService.OnGamePaused().Subscribe(OnGamePaused);
        }

        protected virtual void OnGamePaused(bool isPaused){}

        protected virtual void OnDestroy(){}
    }
}