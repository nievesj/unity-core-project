using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Service;
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
		protected ServiceFramework serviceFramework;
		protected AssetBundleCreateRequest bundleRequest;
		protected Hash128 Hash { get { return default(Hash128); } }
		protected Dictionary<string, LoadedBundle> downloadedBundles;
		protected Dictionary<string, UnityEngine.Object> loadedAssets;
		protected Dictionary<string, UnityEngine.Object> editorAssets;

		public AssetBundleLoader(IAssetService service, ServiceFramework app)
		{
			assetService = service as AssetService;
			serviceFramework = app;

			downloadedBundles = new Dictionary<string, LoadedBundle>();
			loadedAssets = new Dictionary<string, UnityEngine.Object>();
		}

		public void GetSingleAsset(BundleNeeded bundleNeeded, System.Action<UnityEngine.Object> callback)
		{
			GetSingleAsset<UnityEngine.Object>(bundleNeeded, callback);
		}

		public void GetSingleAsset<T>(BundleNeeded bundleNeeded, System.Action<T> callback) where T : UnityEngine.Object
		{
			System.Action<List<T>> returnCallback = bundle =>
			{
				if (bundle != null && bundle.Count > 0)
					callback(bundle.First());
				else
					Debug.LogError("Bundle not found. " + bundleNeeded.AssetName);
			};

#if UNITY_EDITOR
			if (EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES)
			{
				SimulateAssetBundle<T>(bundleNeeded, returnCallback);

				return;
			}
#endif

			serviceFramework.StartCoroutine(GetBundle<T>(bundleNeeded, returnCallback));
		}

		public void GetAllAssets(BundleNeeded bundleNeeded, System.Action<List<UnityEngine.Object>> callback)
		{
			GetAllAssets<UnityEngine.Object>(bundleNeeded, callback);
		}

		public void GetAllAssets<T>(BundleNeeded bundleNeeded, System.Action<List<T>> callback) where T : UnityEngine.Object
		{
			serviceFramework.StartCoroutine(DownloadBundleManifest<T>(bundleNeeded, callback));
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

#if UNITY_EDITOR
		protected void SimulateAssetBundle<T>(BundleNeeded bundleNeeded, System.Action<List<T>> callback) where T : UnityEngine.Object
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
			}

			callback(assets);
		}
#endif

		protected IEnumerator DownloadBundleManifest<T>(BundleNeeded bundleNeeded, System.Action<List<T>> callback) where T : UnityEngine.Object
		{
			yield return GetBundle<T>(bundleNeeded, callback);
		}

		protected IEnumerator GetBundle<T>(BundleNeeded bundleNeeded, System.Action<List<T>> callback) where T : UnityEngine.Object
		{
			if (!assetService.UseStreamingAssets)
			{
				yield return GetBundleFromWebOrCache<T>(bundleNeeded, callback);
			}
			else
			{
				yield return GetBundleFromStreamingAssets<T>(bundleNeeded, callback);
			}
		}

		protected IEnumerator GetBundleFromWebOrCache<T>(BundleNeeded bundleNeeded, System.Action<List<T>> callback) where T : UnityEngine.Object
		{
			UnityWebRequest www = null;
			AssetBundle bundle;

			Debug.Log(("AssetBundleLoader: " + bundleNeeded.Options.AssetCacheState + " | Requesting: " + bundleNeeded.AssetName + " | " + bundleNeeded.BundleName).Colored(Colors.aqua));

			//look at this again, I might need to keep the crc and hash, at least just the hash from the server..
			//https://github.com/UnityCommunity/UnityLibrary/blob/master/Assets/Scripts/AssetBundles/AssetBundleLoader.cs

			switch (bundleNeeded.Options.AssetCacheState)
			{
				case AssetCacheState.Cache:
					www = UnityWebRequest.GetAssetBundle(GetAssetPath(bundleNeeded), Hash, 0);
					break;
				case AssetCacheState.NoCache:
					www = UnityWebRequest.GetAssetBundle(GetAssetPath(bundleNeeded));
					break;
			}

			yield return www.SendWebRequest();

			bundle = DownloadHandlerAssetBundle.GetContent(www);

			if (www.isNetworkError)
				Debug.LogError("AssetBundleLoader: Can't load asset bundle: " + www.error);
			else
				ProcessDownloadedBundle<T>(bundleNeeded, new LoadedBundle(bundle), callback);

			www.Dispose();
		}

		protected IEnumerator GetBundleFromStreamingAssets<T>(BundleNeeded bundleNeeded, System.Action<List<T>> callback) where T : UnityEngine.Object
		{
			Debug.Log(("AssetBundleLoader: Using StreamingAssets - " + " Requesting:" + bundleNeeded.AssetName + " | " + bundleNeeded.BundleName).Colored(Colors.aqua));

			var bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, GetAssetPathFromLocalStreamingAssets(bundleNeeded)));
			yield return bundleLoadRequest;

			if (bundleLoadRequest.assetBundle == null)
				Debug.LogError("AssetBundleLoader: Failed to load AssetBundle!");
			else
				ProcessDownloadedBundle<T>(bundleNeeded, new LoadedBundle(bundleLoadRequest.assetBundle), callback);
		}

		protected string GetAssetPath(BundleNeeded bundleNeeded)
		{
			if (bundleNeeded.AssetCategory.Equals(AssetCategoryRoot.None))
				return assetService.AssetBundlesURL + bundleNeeded.BundleName + "?r=" + (Random.value * 9999999); //this random value prevents caching on the web server
			else
				return assetService.AssetBundlesURL + bundleNeeded.AssetCategory.ToString().ToLower() + "/" + bundleNeeded.BundleName;
		}

		protected string GetManifestPath(BundleNeeded bundleNeeded)
		{
			if (bundleNeeded.AssetCategory.Equals(AssetCategoryRoot.None))
				return assetService.AssetBundlesURL + bundleNeeded.ManifestName;
			else
				return assetService.AssetBundlesURL + bundleNeeded.AssetCategory.ToString().ToLower() + "/" + bundleNeeded.ManifestName;
		}

		protected string GetAssetPathFromLocalStreamingAssets(BundleNeeded bundleNeeded)
		{
			if (bundleNeeded.AssetCategory.Equals(AssetCategoryRoot.None))
				return bundleNeeded.BundleName;
			else
				return bundleNeeded.AssetCategory.ToString().ToLower() + "/" + bundleNeeded.BundleName;
		}

		protected string GetAssetPathFromLocalStreamingAssetsManifest(BundleNeeded bundleNeeded)
		{
			if (bundleNeeded.AssetCategory.Equals(AssetCategoryRoot.None))
				return bundleNeeded.ManifestName;
			else
				return bundleNeeded.AssetCategory.ToString().ToLower() + "/" + bundleNeeded.ManifestName;
		}

		protected void ProcessDownloadedBundle<T>(BundleNeeded bundleNeeded, LoadedBundle bundle, System.Action<List<T>> callback) where T : UnityEngine.Object
		{
			if (!downloadedBundles.ContainsKey(bundleNeeded.BundleName))
				downloadedBundles.Add(bundleNeeded.BundleName, bundle);

			switch (bundleNeeded.Options.AssetLoadProcess)
			{
				case AssetLoadProcess.LoadSingleAsync:
					serviceFramework.StartCoroutine(bundle.LoadAssetAsync<T>(bundleNeeded.AssetName, callback));
					break;
				case AssetLoadProcess.LoadSingleSync:
					bundle.LoadAssetSync<T>(bundleNeeded.AssetName, callback);
					break;
				case AssetLoadProcess.LoadAllAsync:
					serviceFramework.StartCoroutine(bundle.LoadAllAssetsAsync<T>(callback));
					break;
				case AssetLoadProcess.LoadAllSync:
					bundle.LoadAllAssetsSync<T>(callback);
					break;
			}
		}
	}
}