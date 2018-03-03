using System;
using System.Collections.Generic;
using Core.Services.Assets;
using Core.Services.Audio;
using Core.Services.Levels;
using Core.Services.Scenes;
using Core.Services.UI;
using Core.Services.UpdateManager;
using UniRx;
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
		public CoreFrameworkInstaller GameInstaller;

		public override void InstallBindings()
		{
			Container.BindInstance(GameInstaller);

			if (disableLogging)
				Debug.unityLogger.logEnabled = false;

			//TODO: JMN I'm not convinced this is the BEST way to go about this, but it will work for now.
			Action<ConfigurationServiceName> onServiceCreated = configServiceName =>
			{
				if (configServiceName.service is IAssetService)
				{
					Container.BindInstance<IAssetService>((IAssetService)configServiceName.service).AsSingle();
					Container.QueueForInject((IAssetService)configServiceName.service);

					Container.Bind<AssetBundleLoader>().AsSingle().WithArguments((IAssetService)configServiceName.service);

					//Container.BindInstance<IAssetService>((IAssetService)configServiceName.service).AsSingle();
				}

				if (configServiceName.service is IAudioService)
				{
					Container.BindInstance<IAudioService>((IAudioService)configServiceName.service).AsSingle();
					Container.QueueForInject((IAudioService)configServiceName.service);
				}

				if (configServiceName.service is ILevelLoaderService)
				{
					Container.BindInstance<ILevelLoaderService>((ILevelLoaderService)configServiceName.service).AsSingle();
					Container.QueueForInject((ILevelLoaderService)configServiceName.service);
				}

				if (configServiceName.service is ISceneLoaderService)
				{
					Container.BindInstance<ISceneLoaderService>((ISceneLoaderService)configServiceName.service).AsSingle();
					Container.QueueForInject((ISceneLoaderService)configServiceName.service);
				}

				if (configServiceName.service is IUIService)
				{
					Container.BindInstance<IUIService>((IUIService)configServiceName.service).AsSingle();
					Container.QueueForInject((IUIService)configServiceName.service);
				}

				if (configServiceName.service is IUpdateService)
				{
					Container.BindInstance<IUpdateService>((IUpdateService)configServiceName.service).AsSingle();
					Container.QueueForInject((IUpdateService)configServiceName.service);
				}
			};

			Debug.Log(("GameConfiguration: Starting Services").Colored(Colors.Lime));
			foreach (var service in services)
			{
				Debug.Log(("--- Starting Service: " + service.name).Colored(Colors.Cyan));
				service.CreateService().Subscribe(onServiceCreated);
			}
		}
	}
}