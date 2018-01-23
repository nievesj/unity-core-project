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

		AssetCacheState AssetCacheState { get; }

		IObservable<UnityEngine.Object> GetAndLoadAsset<T>(BundleNeeded bundleNeeded) where T : UnityEngine.Object;

		void UnloadAsset(string name, bool unloadAllDependencies);
	}

	public class AssetService : IAssetService
	{
		protected AssetServiceConfiguration configuration;
		protected ServiceLocator app;

		public uint AssetBundleVersionNumber { get { return 1; } }

		public string AssetBundlesURL { get { return configuration.AssetBundlesURL + AssetBundleUtilities.ClientPlatform + "/"; } }

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
		}

		public void StartService(ServiceLocator application)
		{
			app = application;
			assetBundlebundleLoader = new AssetBundleLoader(this, app);

			ServiceLocator.OnGameStart.Subscribe(OnGameStart);
			serviceStarted.OnNext(this);
		}

		public void StopService(ServiceLocator application)
		{
			serviceStopped.OnNext(this);

			serviceConfigured.Dispose();
			serviceStarted.Dispose();
			serviceStopped.Dispose();
		}

		public IObservable<UnityEngine.Object> GetAndLoadAsset<T>(BundleNeeded bundleNeeded) where T : UnityEngine.Object
		{
			return assetBundlebundleLoader.GetSingleAsset<T>(bundleNeeded);
		}

		public void UnloadAsset(string name, bool unloadAllDependencies)
		{
			assetBundlebundleLoader.UnloadAsset(name, unloadAllDependencies);
		}

		protected void OnGameStart(ServiceLocator application)
		{
			LoadGameBundle();
		}

		protected void LoadGameBundle()
		{
			BundleNeeded game = new BundleNeeded(AssetCategoryRoot.None, AssetBundleUtilities.ClientPlatform.ToString(), AssetBundleUtilities.ClientPlatform.ToString());
			Resources.UnloadUnusedAssets();

			if (!configuration.UseCache)
				Caching.ClearCache();

			// GetAndLoadAsset<AssetBundleManifest>(game)
			// 	.Subscribe(loadedObject =>
			// 	{
			// 		Debug.Log(("AssetService: Loaded Manifest - " + loadedObject.name).Colored(Colors.lightblue));
			// 		// ManifestInfo info = new ManifestInfo(www.downloadHandler.text);

			// 		//TODO: check dependencies and load anything that has not been loaded thus far ...
			// 	});
		}
	}
}