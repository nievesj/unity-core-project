using System;
using System.Collections;
using System.Collections.Generic;
using Core.Services;
using Core.Services.Assets;
using Core.Services.UI;
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
		/// Attemps to load a level. First the screen is faded
		/// </summary>
		/// <param name="name">bundle name</param>
		/// <returns>Observable</returns>
		public IObservable<Level> LoadLevel(string name)
		{
			//Fade screen before loading level
			return Observable.Create<Level>(
				(IObserver<Level> observer)=>
				{
					var subject = new Subject<Level>();
					Action<UIElement> OnScreenFadeOn = element =>
					{
						Action<Level> OnLevelLoaded = loadedLevel =>
						{
							observer.OnNext(loadedLevel);
							observer.OnCompleted();
						};

						DoLoadLevel(name).Subscribe(OnLevelLoaded);
					};

					//Start fade screen
					uiService.DarkenScreen(true).Subscribe(OnScreenFadeOn);

					return subject.Subscribe();
				});
		}

		/// <summary>
		/// Once the screen has been blocked, load the level
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private IObservable<Level> DoLoadLevel(string name)
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

						//Level loaded, return screen to normal.
						uiService.DarkenScreen(false).Subscribe();

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