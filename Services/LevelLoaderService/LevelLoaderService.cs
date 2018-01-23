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
		void UnloadLevel(Level level);
		void LoadLevel(Levels level);
		void LoadLevel(string name);

		Level CurrentLevel { get; }

		IObservable<Level> OnLevelLoaded { get; }
		IObservable<Level> OnLevelUnloaded { get; }
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
		protected CompositeDisposable disposables = new CompositeDisposable();

		protected Subject<IService> serviceConfigured = new Subject<IService>();
		public IObservable<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Subject<IService> serviceStarted = new Subject<IService>();
		public IObservable<IService> ServiceStarted { get { return serviceStarted; } }

		protected Subject<IService> serviceStopped = new Subject<IService>();
		public IObservable<IService> ServiceStopped { get { return serviceStopped; } }

		protected Subject<Level> onLevelLoaded = new Subject<Level>();
		public IObservable<Level> OnLevelLoaded { get { return onLevelLoaded; } }

		protected Subject<Level> onLevelUnloaded = new Subject<Level>();
		public IObservable<Level> OnLevelUnloaded { get { return onLevelUnloaded; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as LevelLoaderServiceConfiguration;

			serviceConfigured.OnNext(this);
		}

		public void StartService(ServiceLocator application)
		{
			app = application;
			serviceStarted.OnNext(this);

			ServiceLocator.OnGameStart.Subscribe(OnGameStart);
		}

		public void StopService(ServiceLocator application)
		{
			serviceStopped.OnNext(this);

			serviceConfigured.Dispose();
			serviceStarted.Dispose();
			serviceStopped.Dispose();

			onLevelLoaded.Dispose();
			onLevelUnloaded.Dispose();

			disposables.Dispose();
		}

		protected void OnGameStart(ServiceLocator application)
		{
			uiService = ServiceLocator.GetService<IUIService>();
			assetService = ServiceLocator.GetService<IAssetService>() as AssetService;

			//Load first level
			if (configuration.levels != null && configuration.levels.Count > 0)
				LoadLevel(configuration.levels[0]);
			else
				Debug.LogError("LevelLoaderService: No levels configured");
		}

		protected void OnLoadingWindowOpened(UIWindow window)
		{
			if (window is UILoadingWindow)
			{
				loadingScreen = window;
				Load(currentLevelName);
			}
		}

		protected void OnLoadingWindowClosed(UIWindow window)
		{
			disposables.Dispose();
		}

		public void LoadLevel(Levels level)
		{
			LoadLevel(level.ToString());
		}

		public void LoadLevel(string name)
		{
			if (currentLevel)
			{
				UnloadLevel(currentLevel);
			}

			currentLevelName = name;
			if (uiService != null)
			{
				//not entirely sure this is ok, but works for now. Might be a problem with stacked windows.
				//once I get more familiar with Rx I will revise this.
				if (disposables.IsDisposed)
					disposables = new CompositeDisposable();

				uiService.OnWindowOpened
					.Subscribe(OnLoadingWindowOpened)
					.AddTo(disposables);

				uiService.OnWindowClosed
					.Subscribe(OnLoadingWindowClosed)
					.AddTo(disposables);;

				uiService.Open(UIWindows.UILoading);
			}
			else
				Load(currentLevelName);
		}

		protected void Load(string name)
		{
			BundleNeeded level = new BundleNeeded(AssetCategoryRoot.Levels, name.ToLower(), name.ToLower());
			Resources.UnloadUnusedAssets();

			assetService.GetAndLoadAsset<Level>(level)
				.Subscribe(loadedLevel =>
				{
					Debug.Log(("LevelLoaderService: Loaded level - " + loadedLevel.name).Colored(Colors.lightblue));

					currentLevel = GameObject.Instantiate<Level>(loadedLevel as Level);

					// currentLevel = ob.GetComponent<Level>();

					currentLevel.name = loadedLevel.name;

					currentLevel.OnLevelCompleted.Subscribe(UnloadLevel);

					if (loadingScreen)
						loadingScreen.Close();

					onLevelLoaded.OnNext(currentLevel);
				});
		}

		public void UnloadLevel(Level level)
		{
			if (level)
			{
				Debug.Log(("LevelLoaderService: Unloading level  - " + currentLevel.name).Colored(Colors.lightblue));
				GameObject.Destroy(level.gameObject);
				assetService.UnloadAsset(level.LevelName, true);
			}

			Debug.Log(("LevelLoaderService: Releasing unused resources").Colored(Colors.lightblue));
			Resources.UnloadUnusedAssets();

			onLevelUnloaded.OnNext(null);
		}
	}
}