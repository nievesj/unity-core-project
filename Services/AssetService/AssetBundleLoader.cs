using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Service;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Assets
{
	/// <summary>
	/// Utility class to load asset bundles. Can load bundles from web or from the streaming assets folder.
	/// </summary>
	public class AssetBundleLoader
	{
		protected AssetService assetService;
		protected AssetBundleCreateRequest bundleRequest;
		protected Dictionary<string, LoadedBundle> downloadedBundles;
		protected Dictionary<string, UnityEngine.Object> loadedAssets;

		public AssetBundleLoader(IAssetService service)
		{
			assetService = service as AssetService;

			downloadedBundles = new Dictionary<string, LoadedBundle>();
			loadedAssets = new Dictionary<string, UnityEngine.Object>();
		}

		public IObservable<T> LoadAsset<T>(BundleRequest bundleNeeded)where T : UnityEngine.Object
		{
			return GetBundle<T>(bundleNeeded);
		}

		public void UnloadAsset(string name, bool unloadAllDependencies)
		{
			name = name.ToLower();

			if (downloadedBundles.ContainsKey(name))
			{
				downloadedBundles[name].Unload(unloadAllDependencies);
				downloadedBundles.Remove(name);
			}
		}

		protected IObservable<T> GetBundle<T>(BundleRequest bundleNeeded)where T : UnityEngine.Object
		{
#if UNITY_EDITOR
			if (EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES)
			{
				return Observable.FromCoroutine<T>((observer, cancellationToken)=> SimulateAssetBundle<T>(bundleNeeded, observer, cancellationToken));
			}
#endif
			if (!assetService.UseStreamingAssets)
			{
				return Observable.FromCoroutine<T>((observer, cancellationToken)=> GetBundleFromWebOrCacheOperation<T>(bundleNeeded, observer, cancellationToken));
			}
			else
			{
				return GetBundleFromStreamingAssets<T>(bundleNeeded);
			}
		}

#if UNITY_EDITOR
		protected IEnumerator SimulateAssetBundle<T>(BundleRequest bundleNeeded, IObserver<T> observer, CancellationToken cancellationToken)where T : UnityEngine.Object
		{
			Debug.Log(("AssetBundleLoader: Simulated | Requesting: " + bundleNeeded.AssetName + " | " + bundleNeeded.BundleName).Colored(Colors.aqua));

			List<T> assets = new List<T>();
			var guids = UnityEditor.AssetDatabase.FindAssets(bundleNeeded.BundleName);

			foreach (var id in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(id);
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
				if (asset && asset.name.ToLower().Equals(bundleNeeded.BundleName.ToLower()))
				{
					assets.Add(asset);
					break;
				}

				yield return null;
			}

			observer.OnNext(assets.First());
			observer.OnCompleted();
		}
#endif

		protected IEnumerator GetBundleFromWebOrCacheOperation<T>(BundleRequest bundleNeeded, IObserver<T> observer, CancellationToken cancellationToken)where T : UnityEngine.Object
		{
			UnityWebRequest www = null;
			ManifestInfo manifestInfo = new ManifestInfo(bundleNeeded);
			AssetBundle bundle;

			Debug.Log(("AssetBundleLoader: " + bundleNeeded.AssetCacheState + " | Requesting: " + bundleNeeded.AssetName + " | " + bundleNeeded.BundleName).Colored(Colors.aqua));

			if (bundleNeeded.AssetCacheState.Equals(bundleNeeded.AssetCacheState))
			{
				yield return manifestInfo.GetInfo().ToYieldInstruction();

				www = UnityWebRequest.GetAssetBundle(bundleNeeded.AssetPath, manifestInfo.Hash, 0);
			}
			else
			{
				www = UnityWebRequest.GetAssetBundle(bundleNeeded.AssetPath);
			}

			yield return www.SendWebRequest();

			bundle = DownloadHandlerAssetBundle.GetContent(www);

			if (www.isNetworkError)
			{
				observer.OnError(new System.Exception("AssetBundleLoader: " + www.error));
			}
			else
			{
				ProcessDownloadedBundle<T>(bundleNeeded, new LoadedBundle(bundle, manifestInfo))
					.Subscribe(xs =>
					{
						observer.OnNext(xs);
						observer.OnCompleted();
					});
			}

			www.Dispose();
		}

		protected IObservable<T> GetBundleFromStreamingAssets<T>(BundleRequest bundleNeeded)where T : UnityEngine.Object
		{
			Debug.Log(("AssetBundleLoader: Using StreamingAssets - " + " Requesting:" + bundleNeeded.AssetCategory + " | " + bundleNeeded.BundleName).Colored(Colors.aqua));
			string path = Path.Combine(Application.streamingAssetsPath, bundleNeeded.AssetPathFromLocalStreamingAssets);

			return Observable.FromCoroutine<T>((observer, cancellationToken)=> RunAssetBundleCreateRequestOperation<T>(AssetBundle.LoadFromFileAsync(path), bundleNeeded, observer, cancellationToken));
		}

		protected IEnumerator RunAssetBundleCreateRequestOperation<T>(UnityEngine.AssetBundleCreateRequest assetBundleCreateRequest, BundleRequest bundleNeeded, IObserver<T> observer, CancellationToken cancellationToken)where T : UnityEngine.Object
		{
			while (!assetBundleCreateRequest.isDone && !cancellationToken.IsCancellationRequested)
				yield return null;

			ProcessDownloadedBundle<T>(bundleNeeded, new LoadedBundle(assetBundleCreateRequest.assetBundle))
				.Subscribe(xs =>
				{
					if (!xs)
						observer.OnError(new System.Exception("RunAssetBundleCreateRequestOperation: Error getting bundle " + bundleNeeded.AssetName));

					observer.OnNext(xs);
					observer.OnCompleted();
				});
		}

		protected IObservable<T> ProcessDownloadedBundle<T>(BundleRequest bundleNeeded, LoadedBundle bundle)where T : UnityEngine.Object
		{
			if (!downloadedBundles.ContainsKey(bundleNeeded.BundleName))
				downloadedBundles.Add(bundleNeeded.BundleName, bundle);

			return bundle.LoadAssetAsync<T>(bundleNeeded.AssetName);
		}
	}
}