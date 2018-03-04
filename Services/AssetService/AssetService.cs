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
	public class AssetService : Service
	{
		[Inject]
		private AssetBundleLoader assetBundlebundleLoader;

		public bool UseStreamingAssets { get { return Configuration.UseStreamingAssets; } }

		public AssetCacheState AssetCacheState { get { return Configuration.UseCache ? AssetCacheState.Cache : AssetCacheState.NoCache; } }

		public AssetCacheStrategy AssetCacheStrategy { get { return Configuration.CacheBundleManifestsLocally ? AssetCacheStrategy.CopyBundleManifestFileLocally : AssetCacheStrategy.UseUnityCloudManifestBuildVersion; } }

		private UnityCloudBuildManifest _cloudBuildManifest;

		public UnityCloudBuildManifest CloudBuildManifest { get { return _cloudBuildManifest; } }

		public readonly AssetServiceConfiguration Configuration;

		public AssetService(ServiceConfiguration config)
		{
			Configuration = config as AssetServiceConfiguration;

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
	}
}