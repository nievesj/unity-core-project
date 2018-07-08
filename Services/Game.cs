using System;
using Core.Services.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services
{
    /// <summary>
    /// Starting point for Core Framework.
    /// </summary>
    public abstract class Game : CoreBehaviour
    {
        [Inject]
        private SignalBus _signalBus;

        protected override void Awake()
        {
            //Make this object persistent
            DontDestroyOnLoad(gameObject);            
            _signalBus.Subscribe<GameStartedSignal>(OnGameStart);
        }

        /// <summary>
        /// Method triggered when the game starts.
        /// </summary>
        /// <param name="unit"></param>
        protected virtual void OnGameStart()
        {
            _uiService.OnGamePaused().Subscribe(OnGamePaused);
            Debug.Log("Game Started".Colored(Colors.Lime));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _signalBus.Unsubscribe<GameStartedSignal>(OnGameStart);
        }
    }
}