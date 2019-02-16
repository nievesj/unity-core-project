using System;
using System.Threading.Tasks;
using Core.Services.Factory;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public class CoreFrameworkInstaller : MonoInstaller<CoreFrameworkInstaller>
    {
        public override void InstallBindings()
        {
            //nop
        }
    }
}