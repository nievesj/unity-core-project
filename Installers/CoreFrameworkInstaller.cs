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