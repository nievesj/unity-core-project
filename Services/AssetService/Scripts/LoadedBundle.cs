using System.Collections;
using UniRx;
using UnityEngine;

namespace Core.Services.Assets
{
	/// <summary>
	/// Represents a loaded Asset bundle and the operations that can be applied to it
	/// </summary>
	public class LoadedBundle
	{
		private AssetBundle assetBundle;
		private ManifestInfo manifestInfo;

		internal AssetBundle Bundle { get { return assetBundle; } }

		public LoadedBundle(AssetBundle asset)
		{
			assetBundle = asset;
		}

		public LoadedBundle(AssetBundle asset, ManifestInfo info)
		{
			assetBundle = asset;
			manifestInfo = info;
		}

		/// <summary>
		/// Unload bundle. Only use this when the bundle is no longer needed.
		/// </summary>
		/// <param name="unloadAll"></param>
		public void Unload(bool unloadAll = false)
		{
			if (assetBundle)
				assetBundle.Unload(unloadAll);
		}

		/// <summary>
		/// Loads an asset inside this bundle
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IObservable<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
		{
			Debug.Log(("LoadedBundle: Asynchronously loading asset: " + name).Colored(Colors.Yellow));

			//If the asset bundle contains a scene, pass the scene up the stream so it can be loaded
			if (assetBundle.GetAllScenePaths().Length > 0)
				return Observable.FromCoroutine<T>((observer) => RunSceneRequestOperation<T>(assetBundle as T, observer));
			else
				return Observable.FromCoroutine<T>((observer) => RunAssetBundleRequestOperation<T>(assetBundle.LoadAssetAsync(name), observer));
		}

		/// <summary>
		/// Wrapper method to load a scene. We cannot use assetBundle.LoadAssetAsync to load a scene,
		/// therefore a dummy method is needed to pass the scene up the stream
		/// </summary>
		/// <param name="obj">               Scene </param>
		/// <param name="observer">          Observer </param>
		/// <param name="cancellationToken"> Cancellation Token </param>
		/// <returns></returns>
		private IEnumerator RunSceneRequestOperation<T>(T obj, IObserver<T> observer) where T : UnityEngine.Object
		{
			yield return null;

			observer.OnNext(obj);
			observer.OnCompleted();
		}

		/// <summary>
		/// Operation extracts an asset from the loaded bundle
		/// </summary>
		/// <param name="asyncOperation">   </param>
		/// <param name="observer">         </param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private IEnumerator RunAssetBundleRequestOperation<T>(UnityEngine.AssetBundleRequest asyncOperation, IObserver<T> observer) where T : UnityEngine.Object
		{
			yield return asyncOperation;

			if (!asyncOperation.asset)
				observer.OnError(new System.Exception("RunAssetBundleRequestOperation: Error getting bundle."));

			//Current use case returns the component, if this changes then deal with it downstream but for now this should be ok
			var go = asyncOperation.asset as GameObject;
			var comp = go.GetComponent<T>();

			observer.OnNext(comp);
			observer.OnCompleted();
		}
	}
}