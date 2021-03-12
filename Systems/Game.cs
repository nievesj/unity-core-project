using Zenject;

namespace Core.Services
{
    /// <summary>
    /// Starting point for Core Framework.
    /// </summary>
    public abstract class Game : CoreBehaviour
    {
        [Inject] //This is set up on SceneInstaller
        private SignalBus _signalBus;

        protected virtual void Awake()
        {
            //Listen to game lifetime events
            _signalBus.Subscribe<OnGameStartedSignal>(OnGameStartInternal);
            _signalBus.Subscribe<OnGamePaused>(OnGamePausedInternal);
            _signalBus.Subscribe<OnGameResumed>(OnGameResumedInternal);
            _signalBus.Subscribe<OnGameLostFocus>(OnGameLostFocusInternal);
            _signalBus.Subscribe<OnGameGotFocus>(OnGameGotFocusInternal);
            _signalBus.Subscribe<OnGameQuit>(OnGameQuitInternal);
        }

        private void OnGameStartInternal()
        {
            _signalBus.TryUnsubscribe<OnGameStartedSignal>(OnGameStartInternal);

            OnGameStart();
        }

        private void OnGamePausedInternal()
        {
            OnGamePaused(true);
        }

        private void OnGameResumedInternal()
        {
            OnGamePaused(false);
        }

        private void OnGameLostFocusInternal()
        {
            OnGameFocusChange(false);
        }

        private void OnGameGotFocusInternal()
        {
            OnGameFocusChange(true);
        }

        private void OnGameQuitInternal()
        {
            _signalBus.TryUnsubscribe<OnGameQuit>(OnGameStart);

            OnGameQuit();
        }

        protected abstract void OnGameStart();
        protected abstract void OnGamePaused(bool isPaused);
        protected abstract void OnGameFocusChange(bool hasFocus);
        protected abstract void OnGameQuit();
    }
}