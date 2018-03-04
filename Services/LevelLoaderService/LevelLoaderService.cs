using System;
using System.Collections;
using System.Collections.Generic;
using Core.Services;
using Core.Services.Assets;
using Core.Services.Factory;
using Core.Services.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services.Levels
{
	public class LevelLoaderService : Service
	{
		[Inject]
		private AssetService _assetService;

		[Inject]
		private UIService _uiService;

		[Inject]
		private FactoryService _factoryService;

		private LevelLoaderServiceConfiguration _configuration;

		private Level currentLevel;
		public Level CurrentLevel { get { return currentLevel; } }

		public LevelLoaderService(ServiceConfiguration config)
		{
			_configuration = config as LevelLoaderServiceConfiguration;
		}

		/// <summary>
		/// Attemps to load a level. First the screen is faded
		/// </summary>
		/// <param name="name"> bundle name </param>
		/// <returns> Observable </returns>
		public IObservable<Level> LoadLevel(string name)
		{
			//Fade screen before loading level
			return Observable.Create<Level>(
				(IObserver<Level> observer) =>
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
					_uiService.DarkenScreen(true).Subscribe(OnScreenFadeOn);

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
				(IObserver<Level> observer) =>
				{
					Action<Level> OnLevelLoaded = loadedLevel =>
					{
						Resources.UnloadUnusedAssets();
						Debug.Log(("LevelLoaderService: Loaded level - " + loadedLevel.name).Colored(Colors.LightBlue));

						currentLevel = _factoryService.Instantiate<Level>(loadedLevel);
						currentLevel.name = loadedLevel.name;

						//Level loaded, return screen to normal.
						_uiService.DarkenScreen(false).Subscribe();

						observer.OnNext(currentLevel);
						observer.OnCompleted();
					};

					return _assetService.GetAndLoadAsset<Level>(bundleRequest)
						.Subscribe(OnLevelLoaded);
				});
		}

		/// <summary>
		/// Unloads level.
		/// </summary>
		/// <param name="level"> level name </param>
		/// <returns> Observable </returns>
		public IObservable<Level> UnloadLevel(Level level)
		{
			var subject = new Subject<Level>();

			if (level)
			{
				Debug.Log(("LevelLoaderService: Unloading level  - " + currentLevel.name).Colored(Colors.LightBlue));
				GameObject.Destroy(level.gameObject);
				_assetService.UnloadAsset(level.name, true);
			}

			subject.OnNext(null);
			subject.OnCompleted();

			Resources.UnloadUnusedAssets();

			return subject;
		}
	}
}