using Zenject;

namespace Core.Services
{
    public class OnGameStartedSignal { }

    public class OnGamePausedSignal
    {
        public bool IsPaused { get; }

        public OnGamePausedSignal(bool isPaused)
        {
            IsPaused = isPaused;
        }
    }

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
            _signalBus.Fire<OnGameStartedSignal>();
        }
    }
}