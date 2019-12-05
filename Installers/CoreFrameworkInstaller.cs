using Zenject;

namespace Core.Services
{
    public class OnGameStartedSignal { }

    public class OnGamePaused { }

    public class OnGameResumed { }

    public class OnGameGotFocus { }

    public class OnGameLostFocus { }

    public class OnGameQuit { }


    /// <summary>
    /// Entry point for the game.
    /// </summary>
    public class CoreFrameworkInstaller : CoreGameSceneInstaller
    {
        public override void InstallBindings() { }
    }
}