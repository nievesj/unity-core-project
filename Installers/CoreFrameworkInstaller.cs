using System;
using UniRx;
using Zenject;

namespace Core.Services
{
    public class GameStartedSignal{}
    
    /// <summary>
    /// Entry point for the game.
    /// </summary>
    public class CoreFrameworkInstaller : MonoInstaller<CoreFrameworkInstaller>
    {
        [Inject]
        private SignalBus _signalBus;

        public override void InstallBindings()
        {
            // Container.BindInstance(OnGameStarted).AsSingle();
        }

        public override void Start()
        {
            _signalBus.Fire<GameStartedSignal>();
        }
    }
}