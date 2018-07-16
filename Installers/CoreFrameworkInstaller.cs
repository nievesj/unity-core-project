using System;
using UniRx;
using Zenject;

namespace Core.Services
{
    /// <summary>
    /// Entry point for the game.
    /// </summary>
    public class CoreFrameworkInstaller : MonoInstaller<CoreFrameworkInstaller>
    {
        private readonly Subject<Unit> _onGameStart = new Subject<Unit>();
        private IObservable<Unit> OnGameStarted => _onGameStart;

        public override void InstallBindings()
        {
            Container.BindInstance(OnGameStarted).AsSingle();
        }

        public override void Start()
        {
            _onGameStart.OnNext(new Unit());
            _onGameStart.OnCompleted();
        }
    }
}