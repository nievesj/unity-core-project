using System;
using System.Collections;
using System.Collections.Generic;
using Core.Service;
using Core.Signals;
using UnityEngine;

namespace Core.Assets
{
	public interface IAssetService : IService
	{
		string AssetBundlesURL { get; }
		AssetBundleLoader BundleLoader { get; }

		AssetCacheState AssetCacheState { get; }
	}

	public class AssetService : IAssetService
	{
		protected AssetServiceConfiguration configuration;
		protected ServiceFramework app;

		public uint AssetBundleVersionNumber { get { return 1; } }

		public string AssetBundlesURL { get { return configuration.AssetBundlesURL + AssetBundleUtilities.ClientPlatform + "/"; } }

		public bool UseStreamingAssets { get { return configuration.UseStreamingAssets; } }

		public AssetCacheState AssetCacheState { get { return configuration.UseCache ? AssetCacheState.Cache : AssetCacheState.NoCache; } }

		protected AssetBundleLoader assetBundlebundleLoader;
		public AssetBundleLoader BundleLoader { get { return assetBundlebundleLoader; } }

		protected Signal<IService> serviceConfigured = new Signal<IService>();
		public Signal<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Signal<IService> serviceStarted = new Signal<IService>();
		public Signal<IService> ServiceStarted { get { return serviceStarted; } }

		protected Signal<IService> serviceStopped = new Signal<IService>();
		public Signal<IService> ServiceStopped { get { return serviceStopped; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as AssetServiceConfiguration;
			serviceConfigured.Dispatch(this);
		}

		public void StartService(ServiceFramework application)
		{
			app = application;
			assetBundlebundleLoader = new AssetBundleLoader(this, app);
			ServiceFramework.OnGameStart.Add(OnGameStart);

			serviceStarted.Dispatch(this);
		}

		public void StopService(ServiceFramework application)
		{
			serviceStopped.Dispatch(this);
		}

		protected void OnGameStart(ServiceFramework application)
		{
			LoadGameBundle();
		}

		protected void LoadGameBundle()
		{
			BundleNeeded game = new BundleNeeded(AssetCategoryRoot.None, AssetBundleUtilities.ClientPlatform.ToString(), AssetBundleUtilities.ClientPlatform.ToString());
			Resources.UnloadUnusedAssets();

			if (!configuration.UseCache)
				Caching.ClearCache();

			if (!EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES)
			{
				// BundleLoader.GetSingleAsset<AssetBundleManifest>(game, loadedObject =>
				// {

				// 	// Debug.Log(("AssetService: Loaded Manifest - " + loadedObject.name).Colored(Colors.lightblue));
				// 	// ManifestInfo info = new ManifestInfo(www.downloadHandler.text);

				// 	//TODO: check dependencies and load anything that has not been loaded thus far ...
				// });
			}
		}
	}
}