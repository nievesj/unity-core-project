using System;
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
            
            _signalBus.Subscribe<OnGameStartedSignal>(OnGameStart);
        }

        /// <summary>
        /// Method triggered when the game starts.
        /// </summary>
        protected virtual void OnGameStart()
        {
            Debug.Log("Game Started".Colored(Colors.Lime));
        }
    }
}