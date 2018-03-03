using System;
using System.Collections;
using System.Collections.Generic;
using Core.Services;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services.Assets
{
	/// <summary>
	/// Central point to get asset bundles required to run the game. 
	/// </summary>
	public interface IAssetService : IService { }

	public class AssetService : IAssetService
	{
		public string AssetBundlesURL { get { return _configuration.AssetBundlesURL + AssetBundleUtilities.ClientPlatform + "/"; } }
		public int ManifestCacheExpiringPeriodInDays { get { return _configuration.ManifestCachePeriod; } }

		public bool UseStreamingAssets { get { return _configuration.UseStreamingAssets; } }

		public AssetCacheState AssetCacheState { get { return _configuration.UseCache ? AssetCacheState.Cache : AssetCacheState.NoCache; } }
		public AssetCacheStrategy AssetCacheStrategy { get { return _configuration.CacheBundleManifestsLocally ? AssetCacheStrategy.CopyBundleManifestFileLocally : AssetCacheStrategy.UseUnityCloudManifestBuildVersion; } }

		private UnityCloudBuildManifest _cloudBuildManifest;
		public UnityCloudBuildManifest CloudBuildManifest { get { return _cloudBuildManifest; } }

		private AssetServiceConfiguration _configuration;

		[Inject]
		private AssetBundleLoader assetBundlebundleLoader;

		public AssetService(ServiceConfiguration config)
		{
			_configuration = config as AssetServiceConfiguration;

			UnityCloufBuildManifestLoader.LoadBuildManifest().Subscribe(cloudManifest =>
			{
				if (cloudManifest != null)
				{
					Debug.Log(("---- AssetService: Unity Cloud Build Manifest present. Build Version: " + cloudManifest.buildNumber).Colored(Colors.Aqua));
					_cloudBuildManifest = cloudManifest;
				}
				else
				{
					Debug.Log(("---- AssetService: Unity Cloud Build Manifest missing. This is ok. Ignoring.").Colored(Colors.Aqua));
				}
			});
		}

		/// <summary>
		/// Gets and loads the required asset bundle 
		/// </summary>
		/// <param name="bundleRequest"> Bundle to request </param>
		/// <returns> Observable </returns>
		public IObservable<T> GetAndLoadAsset<T>(BundleRequest bundleRequest) where T : UnityEngine.Object
		{
			return assetBundlebundleLoader.LoadAsset<T>(bundleRequest);
		}

		/// <summary>
		/// Unloads asset and removes it from memory 
		/// </summary>
		/// <param name="name">                  Asset name </param>
		/// <param name="unloadAllDependencies"> Unload all dependencies? </param>
		public void UnloadAsset(string name, bool unloadAllDependencies)
		{
			assetBundlebundleLoader.UnloadAsset(name, unloadAllDependencies);
		}

		/// <summary>
		/// Gets a bundle that has been previously loaded and it's stored in memory. 
		/// </summary>
		/// <param name="name"> Asset name </param>
		/// <returns> T </returns>
		public T GetLoadedBundle<T>(string name) where T : UnityEngine.Object
		{
			Debug.Log(assetBundlebundleLoader == null ? true : false);
			return assetBundlebundleLoader.GetLoadedBundle<T>(name);
		}

		private void OnGameStart(ServiceLocator locator)
		{
			//assetBundlebundleLoader = new AssetBundleLoader();

			//TODO: Not needed for now. Can be used later to validate asset bundles and manifests.
			//or to preload all assets
			// LoadGameBundle();
		}
	}
}