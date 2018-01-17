using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Core.Assets
{
	/// <summary>
	/// Class provides a single point for loading and unloading asset bundles.
	/// Each method follows the same patterns as the AssetBundle methods counterparts, but instead 
	/// return the desired objects in a callback.
	/// </summary>
	public class LoadedBundle
	{
		protected AssetBundle assetBundle;
		protected AssetBundleManifest manifest;

		public AssetBundle AssetBundle { get { return assetBundle; } }
		public string Name { get { return assetBundle.name; } }
		public AssetBundleManifest Manifest { get { return manifest; } }

		public LoadedBundle(AssetBundle asset)
		{
			assetBundle = asset;
			manifest = asset.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		}

		public void Unload(bool unloadAll = false)
		{
			if (assetBundle)
				assetBundle.Unload(unloadAll);
		}

		/// <summary>
		/// Load a single asset by name and type and return callback with loaded object
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public void LoadAssetSync<T>(string name, System.Action<List<T>> callback) where T : UnityEngine.Object
		{
			Debug.Log(("LoadedBundle: Synchronously loading asset: " + name).Colored(Colors.yellow));

			List<T> ret = new List<T>();
			ret.Add(assetBundle.LoadAsset<T>(name) as T);

			Unload();

			callback(ret);
		}

		/// <summary>
		/// Load all assets of type T and return callback with loaded objects
		/// </summary>
		/// <returns></returns>
		public void LoadAllAssetsSync<T>(System.Action<List<T>> callback) where T : UnityEngine.Object
		{
			Debug.Log(("LoadedBundle: Synchronously loading assets").Colored(Colors.yellow));

			foreach (var s in assetBundle.GetAllAssetNames())
				Debug.Log(("---: " + s).Colored(Colors.yellow));

			Unload();

			callback(assetBundle.LoadAllAssets<T>().ToList());
		}

		/// <summary>
		/// Load asset of type T asynchronously, then return callback with loaded object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public IEnumerator LoadAssetAsync<T>(string name, System.Action<List<T>> callback) where T : UnityEngine.Object
		{
			Debug.Log(("LoadedBundle: Asynchronously loading asset: " + name).Colored(Colors.yellow));

			AssetBundleRequest request = assetBundle.LoadAssetAsync(name);
			yield return request;

			if (request.asset)
			{
				GameObject go = request.asset as GameObject;
				var r = go.GetComponent<T>();
				List<T> ret = new List<T>();
				ret.Add(r);

				Unload();
				callback(ret);
			}
		}

		public IObservable<UnityEngine.Object> LoadAssetAsyncRx<T>(string name) where T : UnityEngine.Object
		{
			Debug.Log(("LoadedBundle: Asynchronously loading asset: " + name).Colored(Colors.yellow));

			return assetBundle.LoadAssetAsync(name)
				.ToObservable()
				.Do(x => Debug.Log(x)) // output progress
				.Last() // last sequence is load completed
				.Subscribe();

			// if (request.asset)
			// {
			// 	GameObject go = request.asset as GameObject;
			// 	var r = go.GetComponent<T>();
			// 	List<T> ret = new List<T>();
			// 	ret.Add(r);

			// 	Unload();
			// 	// callback(ret);
			// }
		}

		/// <summary>
		/// Load all assets of type T asynchronously, then return callback with loaded objects
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		public IEnumerator LoadAllAssetsAsync<T>(System.Action<List<T>> callback) where T : UnityEngine.Object
		{
			Debug.Log(("LoadedBundle: Asynchronously loading assets").Colored(Colors.yellow));

			foreach (var s in assetBundle.GetAllAssetNames())
				Debug.Log(("---: " + s).Colored(Colors.yellow));

			AssetBundleRequest request = assetBundle.LoadAllAssetsAsync();
			yield return request;

			List<T> ret = new List<T>();
			foreach (var ass in request.allAssets)
			{
				GameObject go = request.asset as GameObject;
				var r = go.GetComponent<T>();
				ret.Add(r);
			}

			Unload();
			callback(ret);
		}
	}
}