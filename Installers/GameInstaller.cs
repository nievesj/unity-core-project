using Zenject;

namespace Core.Services
{
    /// <summary>
    /// Entry point for the game.
    /// </summary>
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [Inject]
        private SignalBus _signalBus;

        // private readonly Subject<Unit> _onGameStart = new Subject<Unit>();
        // private IObservable<Unit> OnGameStarted => _onGameStart;

        public override void InstallBindings()
        {
            // Container.BindInstance(OnGameStarted).AsSingle();
        }

        public override void Start()
        {
            _signalBus.Fire<OnGameStartedSignal>();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                _signalBus.Fire<OnGamePaused>();
            else
                _signalBus.Fire<OnGameResumed>();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                _signalBus.Fire<OnGameGotFocus>();
            else
                _signalBus.Fire<OnGameLostFocus>();
        }

        private void OnApplicationQuit()
        {
            _signalBus.Fire<OnGameQuit>();
        }
    }
}