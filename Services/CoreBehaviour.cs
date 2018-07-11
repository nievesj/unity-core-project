using System;
using Core.Services.UI;
using UnityEngine;
using Zenject;

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
        protected UIService _uiService;

        // [Inject]
        // protected SignalBus _signalBus;

        protected virtual void Awake()
        {
            ////Example on how to subscribe to paused signal. Do not uncomment this as all elements
            //// that inherit from CoreBehaviour will trigger it too
            //_signalBus.Subscribe<OnGamePausedSignal>(OnGamePaused);
        }

        protected virtual void Start() { }

        // protected virtual void OnGamePaused(OnGamePausedSignal signal)
        // {
        // 	// Debug.Log($"CoreBehaviour: Game Paused Triggered {name} {signal.IsPaused}".Colored(Colors.LightSalmon));
        // }

        protected virtual void OnDestroy()
        {
            ////Example on how to unsubscribe to paused signal. Do not uncomment this as all elements
            //// that inherit from CoreBehaviour will trigger it too
            // _signalBus.TryUnsubscribe<OnGamePausedSignal>(OnGamePaused);
        }
    }
}