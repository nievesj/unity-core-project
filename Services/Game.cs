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
            
            //Start listening to GameStartedSignal
            _signalBus.Subscribe<OnGameStartedSignal>(OnGameStart);
        }

        /// <summary>
        /// Method triggered when the game starts.
        /// </summary>
        /// <param name="unit"></param>
        protected virtual void OnGameStart()
        {
            Debug.Log("Game Started".Colored(Colors.Lime));
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _signalBus.Unsubscribe<OnGameStartedSignal>(OnGameStart);
        }
    }
}