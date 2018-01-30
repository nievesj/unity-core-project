using System;
using System.Collections;
using System.Collections.Generic;
using Core.Assets;
using Core.Service;
using Core.UI;
using UniRx;
using UnityEngine;

namespace Core.LevelLoaderService
{
	public interface ILevelLoaderService : IService
	{
		IObservable<Level> UnloadLevel(Level level);
		IObservable<Level> LoadLevel(string name);

		Level CurrentLevel { get; }
	}

	public class LevelLoaderService : ILevelLoaderService
	{
		protected LevelLoaderServiceConfiguration configuration;
		protected ServiceLocator app;

		protected AssetService assetService;
		protected IUIService uiService;

		protected Level currentLevel;
		public Level CurrentLevel { get { return currentLevel; } }

		protected string currentLevelName;
		protected UIWindow loadingScreen;

		protected Subject<IService> serviceConfigured = new Subject<IService>();
		public IObservable<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Subject<IService> serviceStarted = new Subject<IService>();
		public IObservable<IService> ServiceStarted { get { return serviceStarted; } }

		protected Subject<IService> serviceStopped = new Subject<IService>();
		public IObservable<IService> ServiceStopped { get { return serviceStopped; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as LevelLoaderServiceConfiguration;

			serviceConfigured.OnNext(this);
			serviceConfigured.OnCompleted();
		}

		public void StartService(ServiceLocator application)
		{
			app = application;
			serviceStarted.OnNext(this);
			serviceStarted.OnCompleted();

			ServiceLocator.OnGameStart.Subscribe(OnGameStart);
		}

		public void StopService(ServiceLocator application)
		{
			serviceStopped.OnNext(this);
			serviceStopped.OnCompleted();
		}

		protected void OnGameStart(ServiceLocator application)
		{
			uiService = ServiceLocator.GetService<IUIService>();
			assetService = ServiceLocator.GetService<IAssetService>()as AssetService;
		}

		public IObservable<Level> LoadLevel(string name)
		{
			if (currentLevel)
				UnloadLevel(currentLevel);

			BundleRequest bundleNeeded = new BundleRequest(AssetCategoryRoot.Levels, name, name);

			return Observable.Create<Level>(
				(IObserver<Level> observer)=>
				{
					Action<Level> OnLevelLoaded = loadedLevel =>
					{
						Resources.UnloadUnusedAssets();
						Debug.Log(("LevelLoaderService: Loaded level - " + loadedLevel.name).Colored(Colors.lightblue));

						currentLevel = GameObject.Instantiate<Level>(loadedLevel);
						currentLevel.name = loadedLevel.name;

						if (loadingScreen)
							loadingScreen.Close();

						observer.OnNext(currentLevel);
						observer.OnCompleted();
					};

					return assetService.GetAndLoadAsset<Level>(bundleNeeded)
						.Subscribe(OnLevelLoaded);
				});
		}

		public IObservable<Level> UnloadLevel(Level level)
		{
			var subject = new Subject<Level>();

			if (level)
			{
				Debug.Log(("LevelLoaderService: Unloading level  - " + currentLevel.name).Colored(Colors.lightblue));
				GameObject.Destroy(level.gameObject);
				assetService.UnloadAsset(level.LevelName, true);
			}

			subject.OnNext(null);
			subject.OnCompleted();

			Resources.UnloadUnusedAssets();

			return subject;
		}
	}
}