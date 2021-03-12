using System.Collections.Generic;
using Core.Common.Extensions.String;
using Core.Systems;
using UnityEngine;
using Zenject;
using Logger = UnityLogger.Logger;

namespace Core.Services
{
    /// <summary>
    /// Entry point for the game.
    /// </summary>
    public class CoreFrameworkInstaller : MonoInstaller<CoreFrameworkInstaller>
    {
        [SerializeField]
        private UnityLogger.LogType debugLevel = UnityLogger.LogType.All;

        [SerializeField, InterfaceType(typeof(ICoreSystem))]
        private List<Object> coreSystems;

        public override void InstallBindings()
        {
            Logger.LogLevel = debugLevel;
            //Initialize SignalBus
            SignalBusInstaller.Install(Container); //This allows SignalBus to be injected in any class instantiated here, or any of its children.
            Logger.Log($"Binding {coreSystems.Count} systems", Colors.Lime);

            //Add Game Scoped Signals
            Container.DeclareSignal<OnGameStartedSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnGamePaused>().OptionalSubscriber();
            Container.DeclareSignal<OnGameResumed>().OptionalSubscriber();
            Container.DeclareSignal<OnGameGotFocus>().OptionalSubscriber();
            Container.DeclareSignal<OnGameLostFocus>().OptionalSubscriber();
            Container.DeclareSignal<OnGameQuit>().OptionalSubscriber();

            //Create system objects and bind them
            foreach (var prefab in coreSystems)
            {
                Container.Bind(prefab.GetType())
                    .FromComponentInNewPrefab(prefab)
                    .AsSingle()
                    .OnInstantiated((InjectContext context, ICoreSystem system) =>
                    {
                        if (system is FactoryCoreSystem factory)
                            factory.SetDiContainer(Container);
                    })
                    .NonLazy();
                Logger.Log($"----{prefab.name}", Colors.Lime);
            }
        }
    }
}