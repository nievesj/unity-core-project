using System.Collections.Generic;
using Core.Common.Extensions.String;
using Core.Systems;
using UnityEngine;
using Zenject;
using Logger = UnityLogger.Logger;

namespace Core.Services
{
    public class OnGameStartedSignal { }

    public class OnGamePaused { }

    public class OnGameResumed { }

    public class OnGameGotFocus { }

    public class OnGameLostFocus { }

    public class OnGameQuit { }

    /// <summary>
    /// Game configuration. Contains the configuration of all the services to be started when the
    /// game starts.
    /// </summary>
    [CreateAssetMenu(fileName = "Game Configuration", menuName = "Installers/Core Framework Settings/Game Configuration")]
    public class GameConfiguration : ScriptableObjectInstaller<GameConfiguration>
    {
        [SerializeField, InterfaceType(typeof(ICoreSystem))]
        private List<Object> coreSystems;

        public override void InstallBindings()
        {
            //Initialize SignalBus
            SignalBusInstaller.Install(Container); //This allows SignalBus to be injected in any class instantiated here, or any of its children.

            //Add Game Scoped Signals
            Container.DeclareSignal<OnGameStartedSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnGamePaused>().OptionalSubscriber();
            Container.DeclareSignal<OnGameResumed>().OptionalSubscriber();
            Container.DeclareSignal<OnGameGotFocus>().OptionalSubscriber();
            Container.DeclareSignal<OnGameLostFocus>().OptionalSubscriber();
            Container.DeclareSignal<OnGameQuit>().OptionalSubscriber();

            Logger.Log($"Binding {coreSystems.Count} systems", Colors.Lime);

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
            
            Logger.Log("GameConfiguration: Service binding complete", Colors.Lime);
        }
    }
}