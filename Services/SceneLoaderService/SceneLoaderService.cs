using System.Collections;
using System.Collections.Generic;
using Core.Assets;
using Core.Service;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Scenes
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

		protected Subject<IService> serviceConfigured = new Subject<IService>();
		public IObservable<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Subject<IService> serviceStarted = new Subject<IService>();
		public IObservable<IService> ServiceStarted { get { return serviceStarted; } }

		protected Subject<IService> serviceStopped = new Subject<IService>();
		public IObservable<IService> ServiceStopped { get { return serviceStopped; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as SceneLoaderServiceConfiguration;

			serviceConfigured.OnNext(this);
			serviceConfigured.OnCompleted();
		}

		public void StartService()
		{
			assetService = ServiceLocator.GetService<IAssetService>();

			serviceStarted.OnNext(this);
			serviceStarted.OnCompleted();
		}

		public void StopService()
		{
			serviceStopped.OnNext(this);
			serviceStopped.OnCompleted();
		}

		public IObservable<UnityEngine.Object> LoadScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
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

		public IObservable<UnityEngine.Object> UnLoadScene(string scene)
		{
			return Observable.Create<UnityEngine.Object>(
				(IObserver<UnityEngine.Object> observer)=>
				{
					SceneManager.UnloadSceneAsync(scene).AsObservable().Subscribe(x =>
					{
						Debug.Log(("SceneLoaderService: Unloaded scene - " + scene).Colored(Colors.lightblue));

						observer.OnNext(null);
						observer.OnCompleted();
					});

					return new Subject<UnityEngine.Object>();
				});
		}
	}
}