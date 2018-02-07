using System;
using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.Assets
{
	public interface IAssetService : IService
	{
		string AssetBundlesURL { get; }
		int ManifestCacheExpiringPeriodInDays { get; }
		bool UseStreamingAssets { get; }

		AssetCacheState AssetCacheState { get; }

		IObservable<T> GetAndLoadAsset<T>(BundleRequest bundleNeeded)where T : UnityEngine.Object;

		T GetLoadedBundle<T>(string name)where T : UnityEngine.Object;

		void UnloadAsset(string name, bool unloadAllDependencies);
	}

	public class AssetService : IAssetService
	{
		protected AssetServiceConfiguration configuration;
		protected AssetBundleLoader assetBundlebundleLoader;

		public uint AssetBundleVersionNumber { get { return 1; } }

		public string AssetBundlesURL { get { return configuration.AssetBundlesURL + AssetBundleUtilities.ClientPlatform + "/"; } }
		public int ManifestCacheExpiringPeriodInDays { get { return configuration.ManifestCachePeriod; } }

		public bool UseStreamingAssets { get { return configuration.UseStreamingAssets; } }

		public AssetCacheState AssetCacheState { get { return configuration.UseCache ? AssetCacheState.Cache : AssetCacheState.NoCache; } }

		public IObservable<IService> Configure(ServiceConfiguration config)
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					configuration = config as AssetServiceConfiguration;
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

					UnityCloufBuildManifestLoader.LoadBuildManifest().Subscribe(cloudManifest =>
					{
						if (cloudManifest != null)
						{
							Debug.Log("Cloud Manifest present");
							Debug.Log(cloudManifest.buildNumber);

						}
						else
						{
							Debug.Log("Cloud Manifest missing - this is not an error");
						}

						observer.OnNext(this);
					});

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

		/// <summary>
		/// Gets and loads the required asset bundle
		/// </summary>
		/// <param name="bundleRequest">Bundle to request</param>
		/// <returns>Observable</returns>
		public IObservable<T> GetAndLoadAsset<T>(BundleRequest bundleRequest)where T : UnityEngine.Object
		{
			return assetBundlebundleLoader.LoadAsset<T>(bundleRequest);
		}

		/// <summary>
		/// Unloads asset and removes it from memory
		/// </summary>
		/// <param name="name">Asset name</param>
		/// <param name="unloadAllDependencies">Unload all dependencies?</param>
		public void UnloadAsset(string name, bool unloadAllDependencies)
		{
			assetBundlebundleLoader.UnloadAsset(name, unloadAllDependencies);
		}

		public T GetLoadedBundle<T>(string name)where T : UnityEngine.Object
		{
			Debug.Log(assetBundlebundleLoader == null?true : false);
			return assetBundlebundleLoader.GetLoadedBundle<T>(name);
		}

		protected void OnGameStart(ServiceLocator application)
		{
			assetBundlebundleLoader = new AssetBundleLoader(this);

			//TODO: Not needed for now. Can be used later to validate asset bundles and manifests. 
			//or to preload all assets
			// LoadGameBundle();
		}

		protected void LoadGameBundle()
		{
			BundleRequest game = new BundleRequest(AssetCategoryRoot.None, AssetBundleUtilities.ClientPlatform.ToString(), AssetBundleUtilities.ClientPlatform.ToString());
			Resources.UnloadUnusedAssets();

			if (!configuration.UseCache)
				Caching.ClearCache();
		}
	}
}