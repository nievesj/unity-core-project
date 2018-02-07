using Core.Services;
using Core.Services.Assets;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Services.Scenes
{
	public interface ISceneLoaderService : IService
	{
		IObservable<UnityEngine.Object> LoadScene(string scene, LoadSceneMode mode);
		IObservable<UnityEngine.Object> UnLoadScene(string scene);
	}

	public class SceneLoaderService : ISceneLoaderService
	{
		protected SceneLoaderServiceConfiguration configuration;
		protected IAssetService assetService;

		public IObservable<IService> Configure(ServiceConfiguration config)
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					configuration = config as SceneLoaderServiceConfiguration;

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

					assetService = ServiceLocator.GetService<IAssetService>();

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

		public IObservable<UnityEngine.Object> LoadScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
		{
			if (assetService.GetLoadedBundle<UnityEngine.Object>(scene))
				return GetPreviouslyLoadedScene(scene, mode);
			else
				return GetScene(scene, mode);
		}

		private IObservable<UnityEngine.Object> GetScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
		{
			BundleRequest bundleNeeded = new BundleRequest(AssetCategoryRoot.Scenes, scene, scene);
			return Observable.Create<UnityEngine.Object>(
				(IObserver<UnityEngine.Object> observer)=>
				{
					System.Action<UnityEngine.Object> OnSceneLoaded = loadedScene =>
					{
						SceneManager.LoadSceneAsync(scene, mode).AsObservable().Subscribe(x =>
						{
							Resources.UnloadUnusedAssets();

							observer.OnNext(loadedScene);
							observer.OnCompleted();

							Debug.Log(("SceneLoaderService: Opened scene - " + scene).Colored(Colors.lightblue));
						});

						Debug.Log(("SceneLoaderService: Loaded scene - " + scene).Colored(Colors.lightblue));
					};
					return assetService.GetAndLoadAsset<UnityEngine.Object>(bundleNeeded).Subscribe(OnSceneLoaded);
				});
		}

		private IObservable<UnityEngine.Object> GetPreviouslyLoadedScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
		{
			var subject = new Subject<UnityEngine.Object>();

			subject.OnError(new System.Exception("Scene " + scene + " is already loaded and open. Opening the same scene twice is not supported."));
			subject.OnCompleted();

			return subject;
		}

		public IObservable<UnityEngine.Object> UnLoadScene(string scene)
		{
			return Observable.Create<UnityEngine.Object>(
				(IObserver<UnityEngine.Object> observer)=>
				{
					SceneManager.UnloadSceneAsync(scene).AsObservable().Subscribe(x =>
					{
						Debug.Log(("SceneLoaderService: Unloaded scene - " + scene).Colored(Colors.lightblue));

						assetService.UnloadAsset(scene, true);

						observer.OnNext(null);
						observer.OnCompleted();
					});

					return new Subject<UnityEngine.Object>();
				});
		}
	}
}