using Core.Services;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Core.Services.Assets
{
	/// <summary>
	/// Utility class to load asset bundles. Can load bundles from web,from the streaming assets
	/// folder, or from the assets folder
	/// </summary>
	public class AssetBundleLoader
	{
		//asset service reference
		[Inject]
		private IAssetService assetService;

		//Keeps track of the bundles that have been loaded
		private Dictionary<string, LoadedBundle> loadedBundles;

		/// <summary>
		/// Initialize object
		/// </summary>
		/// <param name="service"></param>
		internal AssetBundleLoader(IAssetService service)
		{
			loadedBundles = new Dictionary<string, LoadedBundle>();
		}

		/// <summary>
		/// Attemps to load requested asset. Depending on the project options it will look the asset
		/// on the web, on the streaming assets folder or it will attempt to simulate it by loading
		/// it from the asset database. Asset simulation is only available on editor.
		/// </summary>
		/// <param name="bundleRequest"> Bundle to request </param>
		/// <returns> Observable </returns>
		internal IObservable<T> LoadAsset<T>(BundleRequest bundleRequest) where T : UnityEngine.Object
		{
#if UNITY_EDITOR
			if (EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES)
			{
				return Observable.FromCoroutine<T>((observer, cancellationToken) => SimulateAssetBundle<T>(bundleRequest, observer, cancellationToken));
			}
#endif
			Debug.Log("nop");
			if (!assetService.UseStreamingAssets)
			{
				return Observable.FromCoroutine<T>((observer, cancellationToken) => GetBundleFromWebOrCacheOperation<T>(bundleRequest, observer, cancellationToken));
			}
			else
			{
				return GetBundleFromStreamingAssets<T>(bundleRequest);
			}
		}

		/// <summary>
		/// Unloads asset and removes it from memory. Only do this when the asset is no longer needed.
		/// </summary>
		/// <param name="name">                  Asset name </param>
		/// <param name="unloadAllDependencies"> Unload all dependencies? </param>
		internal void UnloadAsset(string name, bool unloadAllDependencies)
		{
			name = name.ToLower();

			if (loadedBundles.ContainsKey(name))
			{
				loadedBundles[name].Unload(unloadAllDependencies);
				loadedBundles.Remove(name);

				Resources.UnloadUnusedAssets();
			}
		}

		internal T GetLoadedBundle<T>(string name) where T : UnityEngine.Object
		{
			if (loadedBundles.ContainsKey(name.ToLower()))
				return loadedBundles[name.ToLower()].Bundle as T;
			else return null;
		}

#if UNITY_EDITOR

		/// <summary>
		/// Method attemps to get an asset from the asset database.
		/// </summary>
		/// <param name="bundleRequest">     Bundle to request </param>
		/// <param name="observer">          Observer </param>
		/// <param name="cancellationToken"> Cancellation token </param>
		/// <returns> IEnumerator </returns>
		private IEnumerator SimulateAssetBundle<T>(BundleRequest bundleRequest, IObserver<T> observer, CancellationToken cancellationToken) where T : UnityEngine.Object
		{
			Debug.Log(("AssetBundleLoader: Simulated | Requesting: " + bundleRequest.AssetName + " | " + bundleRequest.BundleName).Colored(Colors.Aqua));

			List<T> assets = new List<T>();
			//Get guid from the asset
			var guids = UnityEditor.AssetDatabase.FindAssets(bundleRequest.BundleName);

			//This will give you a bunch of assets of the same name, we need to filter it further
			foreach (var id in guids)
			{
				//Get path
				var path = AssetDatabase.GUIDToAssetPath(id);
				//Get actual asset on that path of the type we're looking for.
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
				//This assumes that assets of the same type are located in the same directory, thus, they should have unique names.
				if (asset && asset.name.ToLower().Equals(bundleRequest.BundleName.ToLower()))
				{
					//Break look when the first asset of T is found
					assets.Add(asset);
					break;
				}

				yield return null;
			}

			//Show error if there are duplicate assets on different locations. This should be avoided...
			if (assets.Count > 1)
				Debug.LogError("Duplicate asset names. You have two assets of the same name in different locations. Try using unique names for your assets.");

			observer.OnNext(assets.First());
			observer.OnCompleted();
		}

#endif

		/// <summary>
		/// Method attemps to get a bundle from the web/cloud
		/// </summary>
		/// <param name="bundleRequest">     Bundle to request </param>
		/// <param name="observer">          Observer </param>
		/// <param name="cancellationToken"> Calcellation token </param>
		/// <returns> IEnumerator </returns>
		private IEnumerator GetBundleFromWebOrCacheOperation<T>(BundleRequest bundleRequest, IObserver<T> observer, CancellationToken cancellationToken) where T : UnityEngine.Object
		{
			UnityWebRequest www = null;
			ManifestInfo manifestInfo = new ManifestInfo(bundleRequest);
			AssetBundle bundle;

			Debug.Log(("AssetBundleLoader: " + assetService.AssetCacheState + " | Requesting: " + bundleRequest.AssetName + " | " + bundleRequest.BundleName).Colored(Colors.Aqua));

			//Cache bundles and copy individual bundle .manifest files locally
			if (assetService.AssetCacheState.Equals(AssetCacheState.Cache) && assetService.AssetCacheStrategy.Equals(AssetCacheStrategy.CopyBundleManifestFileLocally))
			{
				//Ok, since we're caching bundles, we need to get the manifest file
				yield return manifestInfo.GetInfo().ToYieldInstruction();

				//Use hash number from the manifest file to determine if UnityWebRequest gets the bundle from web or cache
				www = UnityWebRequest.GetAssetBundle(bundleRequest.AssetPath, manifestInfo.Hash, 0);
			}
			else if (assetService.AssetCacheState.Equals(AssetCacheState.Cache) && assetService.AssetCacheStrategy.Equals(AssetCacheStrategy.UseUnityCloudManifestBuildVersion))
			{
				//cache bundles by using Unity Cloud Build manifest
				uint buildNumber = 0;
				if (assetService.CloudBuildManifest != null)
					buildNumber = System.Convert.ToUInt32(assetService.CloudBuildManifest.buildNumber);

				www = UnityWebRequest.GetAssetBundle(bundleRequest.AssetPath, buildNumber, 0);
			}
			else if (assetService.AssetCacheState.Equals(AssetCacheState.NoCache))
			{
				//No caching, just get the bundle
				www = UnityWebRequest.GetAssetBundle(bundleRequest.AssetPath);
			}

			//Wait until www is done.
			yield return www.SendWebRequest();

			//get bundle
			bundle = DownloadHandlerAssetBundle.GetContent(www);

			if (www.isNetworkError)
			{
				observer.OnError(new System.Exception("AssetBundleLoader: " + www.error));
			}
			else
			{
				//Extract asset from bundle. This assumes that theres is only one asset per bundle. Other use cases will need to modify this.
				ProcessDownloadedBundle<T>(bundleRequest, new LoadedBundle(bundle, manifestInfo))
					.Subscribe(xs =>
					{
						observer.OnNext(xs);
						observer.OnCompleted();
					});
			}

			www.Dispose();
		}

		/// <summary>
		/// Gets bundle from streaming assets directory
		/// </summary>
		/// <param name="bundleRequest"> Bundle to request </param>
		/// <returns> Observable </returns>
		private IObservable<T> GetBundleFromStreamingAssets<T>(BundleRequest bundleRequest) where T : UnityEngine.Object
		{
			Debug.Log(("AssetBundleLoader: Using StreamingAssets - " + " Requesting:" + bundleRequest.AssetCategory + " | " + bundleRequest.BundleName).Colored(Colors.Aqua));
			string path = Path.Combine(Application.streamingAssetsPath, bundleRequest.AssetPathFromLocalStreamingAssets);

			return Observable.FromCoroutine<T>((observer, cancellationToken) => RunAssetBundleCreateRequestOperation<T>(AssetBundle.LoadFromFileAsync(path), bundleRequest, observer, cancellationToken));
		}

		/// <summary>
		/// Operation to get bundle from streaming assets directory
		/// </summary>
		/// <param name="assetBundleCreateRequest"> Asset bundle create request </param>
		/// <param name="bundleRequest">            Bundle to request </param>
		/// <param name="observer">                 observer </param>
		/// <param name="cancellationToken">        Cancellation token </param>
		/// <returns></returns>
		private IEnumerator RunAssetBundleCreateRequestOperation<T>(UnityEngine.AssetBundleCreateRequest assetBundleCreateRequest, BundleRequest bundleRequest, IObserver<T> observer, CancellationToken cancellationToken) where T : UnityEngine.Object
		{
			while (!assetBundleCreateRequest.isDone && !cancellationToken.IsCancellationRequested)
				yield return null;

			ProcessDownloadedBundle<T>(bundleRequest, new LoadedBundle(assetBundleCreateRequest.assetBundle))
				.Subscribe(xs =>
				{
					if (!xs)
						observer.OnError(new System.Exception("RunAssetBundleCreateRequestOperation: Error getting bundle " + bundleRequest.AssetName));

					observer.OnNext(xs);
					observer.OnCompleted();
				});
		}

		/// <summary>
		/// Extracts required asset from the asset bundle
		/// </summary>
		/// <param name="bundleRequest"> Bundle to request </param>
		/// <param name="bundle">        Bundle </param>
		/// <returns></returns>
		private IObservable<T> ProcessDownloadedBundle<T>(BundleRequest bundleRequest, LoadedBundle bundle) where T : UnityEngine.Object
		{
			if (!loadedBundles.ContainsKey(bundleRequest.BundleName))
				loadedBundles.Add(bundleRequest.BundleName, bundle);

			return bundle.LoadAssetAsync<T>(bundleRequest.AssetName);
		}
	}
}