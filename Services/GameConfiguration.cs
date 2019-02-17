using Core.Services.Assets;
using Core.Services.Audio;
using Core.Services.Factory;
using Core.Services.Scenes;
using Core.Services.UI;
using System.Collections.Generic;
using Core.Services.Data;
using Core.Services.Social;
using UnityEngine;
using Zenject;

namespace Core.Services
{
	/// <summary>
	/// Game configuration. Contains the configuration of all the services to be started when the
	/// game starts.
	/// </summary>
	[CreateAssetMenu(fileName = "Game Configuration", menuName = "Installers/Core Framework Settings/Game Configuration")]
	public class GameConfiguration : ScriptableObjectInstaller<GameConfiguration>
	{
		public bool DisableLogging;
		public List<ServiceConfiguration> services = new List<ServiceConfiguration>();

		public override void InstallBindings()
		{
			if (DisableLogging)
				Debug.unityLogger.logEnabled = false;
			
			//Initialize SignalBus
			SignalBusInstaller.Install(Container); //This allows SignalBus to be injected in any class instantiated here, or any of its children.
			
			//Add Game Scoped Signals
			Container.DeclareSignal<OnGameStartedSignal>().OptionalSubscriber();
			Container.DeclareSignal<OnGamePaused>().OptionalSubscriber();
			Container.DeclareSignal<OnGameResumed>().OptionalSubscriber();
			Container.DeclareSignal<OnGameGotFocus>().OptionalSubscriber();
			Container.DeclareSignal<OnGameLostFocus>().OptionalSubscriber();
			Container.DeclareSignal<OnGameQuit>().OptionalSubscriber();
			
			Debug.Log(("GameConfiguration: Starting Services").Colored(Colors.Lime));
			foreach (var service in services)
			{
				Debug.Log(($"--- Starting Service: {service.name}").Colored(Colors.Cyan));

				if (service is AssetServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<AssetService>().AsSingle().WithArguments(service).NonLazy();
					
					Container.Bind<AssetServiceConfiguration>().AsSingle().NonLazy();
					Container.Bind<AssetBundleLoader>().AsSingle().NonLazy();
				}
				else if (service is SceneLoaderServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<SceneLoaderService>().AsSingle().WithArguments(service).NonLazy();
					Container.Bind<SceneLoaderServiceConfiguration>().AsSingle().NonLazy();
				}
				else if (service is UIServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<UIService>().AsSingle().WithArguments(service).NonLazy();
					Container.Bind<UIServiceConfiguration>().AsSingle().NonLazy();
				}
				else if (service is FactoryServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<FactoryService>().AsSingle().WithArguments(service, Container).NonLazy();
					Container.Bind<FactoryServiceConfiguration>().AsSingle().NonLazy();
				}
				else if (service is AudioServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<AudioService>().AsSingle().WithArguments(service).NonLazy();
					Container.Bind<AudioServiceConfiguration>().AsSingle().NonLazy();
				}
				else if (service is SocialServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<SocialService>().AsSingle().WithArguments(service).NonLazy();
					Container.Bind<SocialServiceConfiguration>().AsSingle().NonLazy();
				}
				else if (service is PersistentDataServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<PersistentDataService>().AsSingle().WithArguments(service).NonLazy();
					Container.Bind<PersistentDataServiceConfiguration>().AsSingle().NonLazy();
				}
			}
		}
	}
}