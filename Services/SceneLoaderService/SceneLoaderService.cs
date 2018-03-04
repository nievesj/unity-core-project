using Core.Services.Assets;
using Core.Services.UI;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.Services.Scenes
{
	public class SceneLoaderService : Service
	{
		protected SceneLoaderServiceConfiguration configuration;

		[Inject]
		protected AssetService _assetService;

		[Inject]
		protected UIService _uiService;

		public SceneLoaderService(ServiceConfiguration config)
		{
			configuration = config as SceneLoaderServiceConfiguration;
		}

		/// <summary>
		/// Attempts to load a scene from an asset bundle
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="mode"> </param>
		/// <returns></returns>
		public IObservable<UnityEngine.Object> LoadScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
		{
			//Fade screen before loading level
			return Observable.Create<UnityEngine.Object>(
				(IObserver<UnityEngine.Object> observer) =>
				{
					var subject = new Subject<UnityEngine.Object>();
					Action<UIElement> OnScreenFadeOn = element =>
					{
						Action<UnityEngine.Object> OnSceneLoaded = loadedLevel =>
						{
							observer.OnNext(loadedLevel);
							observer.OnCompleted();
						};

						DoLoadScene(scene, mode).Subscribe(OnSceneLoaded);
					};

					//Start fade screen
					_uiService.DarkenScreen(true).Subscribe(OnScreenFadeOn);

					return subject.Subscribe();
				});
		}

		private IObservable<UnityEngine.Object> DoLoadScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
		{
			if (_assetService.GetLoadedBundle<UnityEngine.Object>(scene))
				return GetPreviouslyLoadedScene(scene, mode);
			else
				return GetScene(scene, mode);
		}

		/// <summary>
		/// Gets a scene from an asset bundle
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="mode"> </param>
		/// <returns></returns>
		private IObservable<UnityEngine.Object> GetScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
		{
			BundleRequest bundleNeeded = new BundleRequest(AssetCategoryRoot.Scenes, scene, scene, _assetService.Configuration);
			return Observable.Create<UnityEngine.Object>(
				(IObserver<UnityEngine.Object> observer) =>
				{
					System.Action<UnityEngine.Object> OnSceneLoaded = loadedScene =>
					{
						SceneManager.LoadSceneAsync(scene, mode).AsObservable().Subscribe(x =>
						{
							Resources.UnloadUnusedAssets();

							//Scene loaded, return screen to normal.
							_uiService.DarkenScreen(false).Subscribe();

							observer.OnNext(loadedScene);
							observer.OnCompleted();

							Debug.Log(("SceneLoaderService: Opened scene - " + scene).Colored(Colors.LightBlue));
						});

						Debug.Log(("SceneLoaderService: Loaded scene - " + scene).Colored(Colors.LightBlue));
					};
					return _assetService.GetAndLoadAsset<UnityEngine.Object>(bundleNeeded).Subscribe(OnSceneLoaded);
				});
		}

		/// <summary>
		/// This is triggered because there was an attempt to load a scene that is currently loaded.
		/// Use case not supported. Calling this method will always trigger an OnError and send it up
		/// the stream.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="mode"> </param>
		/// <returns></returns>
		private IObservable<UnityEngine.Object> GetPreviouslyLoadedScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
		{
			var subject = new Subject<UnityEngine.Object>();

			_uiService.DarkenScreen(false).Subscribe();

			subject.OnError(new System.Exception("Scene " + scene + " is already loaded and open. Opening the same scene twice is not supported."));
			subject.OnCompleted();

			return subject;
		}

		/// <summary>
		/// Unload scene.
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public IObservable<UnityEngine.Object> UnLoadScene(string scene)
		{
			return Observable.Create<UnityEngine.Object>(
				(IObserver<UnityEngine.Object> observer) =>
				{
					SceneManager.UnloadSceneAsync(scene).AsObservable().Subscribe(x =>
					{
						Debug.Log(("SceneLoaderService: Unloaded scene - " + scene).Colored(Colors.LightBlue));

						_assetService.UnloadAsset(scene, true);

						observer.OnNext(null);
						observer.OnCompleted();
					});

					return new Subject<UnityEngine.Object>();
				});
		}
	}
}