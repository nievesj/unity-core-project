using Core.Services.Assets;
using Core.Services.Audio;
using Core.Services.Factory;
using Core.Services.Levels;
using Core.Services.Scenes;
using Core.Services.UI;
using Core.Services.UpdateManager;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.Services
{
	/// <summary>
	/// Game configuration. Contains the configuration of all the services to be started when the
	/// game starts.
	/// </summary>
	[CreateAssetMenu(fileName = "Core Framework", menuName = "Installers/Core Framework Settings/Game Configuration")]
	public class GameConfiguration : ScriptableObjectInstaller<GameConfiguration>
	{
		public bool disableLogging = false;

		public List<ServiceConfiguration> services = new List<ServiceConfiguration>();

		public override void InstallBindings()
		{
			if (disableLogging)
				Debug.unityLogger.logEnabled = false;

			//TODO: JMN I'm not convinced this is the BEST way to go about this, but it will work for now.
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

				if (service is LevelLoaderServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<LevelLoaderService>().AsSingle().WithArguments(service).NonLazy();
					Container.Bind<LevelLoaderServiceConfiguration>().AsSingle().NonLazy();
				}

				if (service is SceneLoaderServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<SceneLoaderService>().AsSingle().WithArguments(service).NonLazy();
					Container.Bind<SceneLoaderServiceConfiguration>().AsSingle().NonLazy();
				}

				if (service is UIServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<UIService>().AsSingle().WithArguments(service).NonLazy();
					Container.Bind<UIServiceConfiguration>().AsSingle().NonLazy();
				}

				if (service is UpdateServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<UpdateService>().AsSingle().WithArguments(service).NonLazy();
					Container.Bind<UpdateServiceConfiguration>().AsSingle().NonLazy();
				}

				if (service is FactoryServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<FactoryService>().AsSingle().WithArguments(service, Container).NonLazy();
					Container.Bind<FactoryServiceConfiguration>().AsSingle().NonLazy();
				}

				if (service is AudioServiceConfiguration)
				{
					Container.BindInterfacesAndSelfTo<AudioService>().AsSingle().WithArguments(service).NonLazy();
					Container.Bind<AudioServiceConfiguration>().AsSingle().NonLazy();
				}
			}
		}
	}
}