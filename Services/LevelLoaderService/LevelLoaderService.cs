using System;
using System.Collections;
using System.Collections.Generic;
using Core.Assets;
using Core.Service;
using Core.Signals;
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

		Signal<Level> OnLevelLoaded { get; }
		Signal<Level> OnLevelUnloaded { get; }
	}

	public class LevelLoaderService : ILevelLoaderService
	{
		protected LevelLoaderServiceConfiguration configuration;
		protected Services app;

		protected AssetService assetService;
		protected IUIService uiService;

		protected Level currentLevel;
		public Level CurrentLevel { get { return currentLevel; } }

		protected string currentLevelName;
		protected UIWindow loadingScreen;

		protected Signal<IService> serviceConfigured = new Signal<IService>();
		public Signal<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Signal<IService> serviceStarted = new Signal<IService>();
		public Signal<IService> ServiceStarted { get { return serviceStarted; } }

		protected Signal<IService> serviceStopped = new Signal<IService>();
		public Signal<IService> ServiceStopped { get { return serviceStopped; } }

		protected Signal<Level> onLevelLoaded = new Signal<Level>();
		public Signal<Level> OnLevelLoaded { get { return onLevelLoaded; } }

		protected Signal<Level> onLevelUnloaded = new Signal<Level>();
		public Signal<Level> OnLevelUnloaded { get { return onLevelUnloaded; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as LevelLoaderServiceConfiguration;

			serviceConfigured.Dispatch(this);
		}

		public void StartService(Services application)
		{
			app = application;
			serviceStarted.Dispatch(this);

			Services.OnGameStart.Add(OnGameStart);
		}

		public void StopService(Services application)
		{
			serviceStopped.Dispatch(this);
		}

		protected void OnGameStart(Services application)
		{
			uiService = Services.GetService<IUIService>();
			assetService = Services.GetService<IAssetService>() as AssetService;

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
			uiService.OnWindowOpened.Remove(OnLoadingWindowOpened);
			uiService.OnWindowClosed.Remove(OnLoadingWindowClosed);
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
				uiService.OnWindowOpened.Add(OnLoadingWindowOpened);
				uiService.OnWindowClosed.Add(OnLoadingWindowClosed);

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

					currentLevel.OnLevelCompleted.Add(UnloadLevel);

					if (loadingScreen)
						loadingScreen.Close();

					onLevelLoaded.Dispatch(currentLevel);
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

			onLevelUnloaded.Dispatch(null);
		}
	}
}