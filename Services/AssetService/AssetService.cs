using System;
using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.Assets
{
	/// <summary>
	/// Central point to get asset bundles required to run the game.
	/// </summary>
	public interface IAssetService : IService
	{
		//Asset bundle location
		string AssetBundlesURL { get; }
		//Time when the manifest files expire
		int ManifestCacheExpiringPeriodInDays { get; }
		//Using streaming assets?
		bool UseStreamingAssets { get; }

		AssetCacheState AssetCacheState { get; }
		AssetCacheStrategy AssetCacheStrategy { get; }
		UnityCloudBuildManifest CloudBuildManifest { get; }

		IObservable<T> GetAndLoadAsset<T>(BundleRequest bundleNeeded)where T : UnityEngine.Object;

		T GetLoadedBundle<T>(string name)where T : UnityEngine.Object;

		void UnloadAsset(string name, bool unloadAllDependencies);
	}

	public class AssetService : IAssetService
	{
		public uint AssetBundleVersionNumber { get { return 1; } }

		public string AssetBundlesURL { get { return configuration.AssetBundlesURL + AssetBundleUtilities.ClientPlatform + "/"; } }
		public int ManifestCacheExpiringPeriodInDays { get { return configuration.ManifestCachePeriod; } }

		public bool UseStreamingAssets { get { return configuration.UseStreamingAssets; } }

		public AssetCacheState AssetCacheState { get { return configuration.UseCache ? AssetCacheState.Cache : AssetCacheState.NoCache; } }
		public AssetCacheStrategy AssetCacheStrategy { get { return configuration.CacheBundleManifestsLocally ? AssetCacheStrategy.CopyBundleManifestFileLocally : AssetCacheStrategy.UseUnityCloudManifestBuildVersion; } }

		private UnityCloudBuildManifest cloudBuildManifest;
		public UnityCloudBuildManifest CloudBuildManifest { get { return cloudBuildManifest; } }

		private AssetServiceConfiguration configuration;
		private AssetBundleLoader assetBundlebundleLoader;

		public IObservable<IService> Configure(ServiceConfiguration config)
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					configuration = config as AssetServiceConfiguration;
					ServiceLocator.OnGameStart.Subscribe(OnGameStart);

					UnityCloufBuildManifestLoader.LoadBuildManifest().Subscribe(cloudManifest =>
					{
						if (cloudManifest != null)
						{
							Debug.Log(("---- AssetService: Unity Cloud Build Manifest present. Build Version: " + cloudManifest.buildNumber).Colored(Colors.Aqua));
							cloudBuildManifest = cloudManifest;
						}
						else
						{
							Debug.Log(("---- AssetService: Unity Cloud Build Manifest missing. This is ok. Ignoring.").Colored(Colors.Aqua));
						}

						observer.OnNext(this);
					});

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

		/// <summary>
		/// Gets a bundle that has been previously loaded and it's stored in memory.
		/// </summary>
		/// <param name="name">Asset name</param>
		/// <returns>T</returns>
		public T GetLoadedBundle<T>(string name)where T : UnityEngine.Object
		{
			Debug.Log(assetBundlebundleLoader == null?true : false);
			return assetBundlebundleLoader.GetLoadedBundle<T>(name);
		}

		private void OnGameStart(ServiceLocator application)
		{
			assetBundlebundleLoader = new AssetBundleLoader(this);

			//TODO: Not needed for now. Can be used later to validate asset bundles and manifests. 
			//or to preload all assets
			// LoadGameBundle();
		}

		/// <summary>
		/// Not used for now.
		/// </summary>
		private void LoadGameBundle()
		{
			BundleRequest game = new BundleRequest(AssetCategoryRoot.None, AssetBundleUtilities.ClientPlatform.ToString(), AssetBundleUtilities.ClientPlatform.ToString());
			Resources.UnloadUnusedAssets();

			if (!configuration.UseCache)
				Caching.ClearCache();
		}
	}
}