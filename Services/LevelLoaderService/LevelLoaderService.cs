using System;
using Core.Services;
using Core.Services.Assets;
using Core.Services.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.Levels
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
		protected IAssetService assetService;
		protected IUIService uiService;
		protected UIWindow loadingScreen;

		protected Level currentLevel;
		public Level CurrentLevel { get { return currentLevel; } }

		public IObservable<IService> Configure(ServiceConfiguration config)
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					configuration = config as LevelLoaderServiceConfiguration;
					ServiceLocator.OnGameStart.Subscribe(OnGameStart);

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		public IObservable<IService> StartService()
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		public IObservable<IService> StopService()
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		protected void OnGameStart(ServiceLocator application)
		{
			uiService = ServiceLocator.GetService<IUIService>();
			assetService = ServiceLocator.GetService<IAssetService>();
		}

		/// <summary>
		/// Attemps to load a level
		/// </summary>
		/// <param name="name">bundle name</param>
		/// <returns>Observable</returns>
		public IObservable<Level> LoadLevel(string name)
		{
			if (currentLevel)
				UnloadLevel(currentLevel);

			BundleRequest bundleRequest = new BundleRequest(AssetCategoryRoot.Levels, name, name);

			return Observable.Create<Level>(
				(IObserver<Level> observer)=>
				{
					Action<Level> OnLevelLoaded = loadedLevel =>
					{
						Resources.UnloadUnusedAssets();
						Debug.Log(("LevelLoaderService: Loaded level - " + loadedLevel.name).Colored(Colors.LightBlue));

						currentLevel = GameObject.Instantiate<Level>(loadedLevel);
						currentLevel.name = loadedLevel.name;

						if (loadingScreen)
							loadingScreen.Close();

						observer.OnNext(currentLevel);
						observer.OnCompleted();
					};

					return assetService.GetAndLoadAsset<Level>(bundleRequest)
						.Subscribe(OnLevelLoaded);
				});
		}

		/// <summary>
		/// Unloads level.
		/// </summary>
		/// <param name="level">level name</param>
		/// <returns>Observable</returns>
		public IObservable<Level> UnloadLevel(Level level)
		{
			var subject = new Subject<Level>();

			if (level)
			{
				Debug.Log(("LevelLoaderService: Unloading level  - " + currentLevel.name).Colored(Colors.LightBlue));
				GameObject.Destroy(level.gameObject);
				assetService.UnloadAsset(level.name, true);
			}

			subject.OnNext(null);
			subject.OnCompleted();

			Resources.UnloadUnusedAssets();

			return subject;
		}
	}
}