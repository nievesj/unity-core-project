using System;
using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UniRx;
using UnityEngine;

namespace Core.Assets
{
	public interface IAssetService : IService
	{
		string AssetBundlesURL { get; }
		int ManifestCacheExpiringPeriodInDays { get; }

		AssetCacheState AssetCacheState { get; }

		IObservable<T> GetAndLoadAsset<T>(BundleRequest bundleNeeded)where T : UnityEngine.Object;

		void UnloadAsset(string name, bool unloadAllDependencies);
	}

	public class AssetService : IAssetService
	{
		protected AssetServiceConfiguration configuration;

		public uint AssetBundleVersionNumber { get { return 1; } }

		public string AssetBundlesURL { get { return configuration.AssetBundlesURL + AssetBundleUtilities.ClientPlatform + "/"; } }
		public int ManifestCacheExpiringPeriodInDays { get { return configuration.ManifestCachePeriod; } }

		public bool UseStreamingAssets { get { return configuration.UseStreamingAssets; } }

		public AssetCacheState AssetCacheState { get { return configuration.UseCache ? AssetCacheState.Cache : AssetCacheState.NoCache; } }

		protected AssetBundleLoader assetBundlebundleLoader;

		protected Subject<IService> serviceConfigured = new Subject<IService>();
		public IObservable<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Subject<IService> serviceStarted = new Subject<IService>();
		public IObservable<IService> ServiceStarted { get { return serviceStarted; } }

		protected Subject<IService> serviceStopped = new Subject<IService>();
		public IObservable<IService> ServiceStopped { get { return serviceStopped; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as AssetServiceConfiguration;
			serviceConfigured.OnNext(this);
			serviceConfigured.OnCompleted();
		}

		public void StartService()
		{
			assetBundlebundleLoader = new AssetBundleLoader(this);

			ServiceLocator.OnGameStart.Subscribe(OnGameStart);
			serviceStarted.OnNext(this);
			serviceStarted.OnCompleted();
		}

		public void StopService()
		{
			serviceStopped.OnNext(this);
			serviceStopped.OnCompleted();
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

		protected void OnGameStart(ServiceLocator application)
		{
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