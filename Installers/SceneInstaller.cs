using Zenject;

namespace Core.Services
{
    /// <summary>
    /// Setup Scene
    /// </summary>
    public class SceneInstaller : MonoInstaller<SceneInstaller>
    {
        [Inject]
        private SignalBus _signalBus;


        public override void InstallBindings(){ }

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