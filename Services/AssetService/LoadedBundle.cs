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

		public void Unload(bool unloadAll = false)
		{
			if (assetBundle)
				assetBundle.Unload(unloadAll);
		}

		public IObservable<T> LoadAssetAsync<T>(string name)where T : UnityEngine.Object
		{
			Debug.Log(("LoadedBundle: Asynchronously loading asset: " + name).Colored(Colors.yellow));

			//If the asset bundle contains a scene, pass the scene up the stream so it can be loaded
			if (assetBundle.GetAllScenePaths().Length > 0)
				return Observable.FromCoroutine<T>((observer, cancellationToken)=> RunSceneRequestOperation<T>(assetBundle as T, observer, cancellationToken));
			else
				return Observable.FromCoroutine<T>((observer, cancellationToken)=> RunAssetBundleRequestOperation<T>(assetBundle.LoadAssetAsync(name), observer, cancellationToken));
		}

		/// <summary>
		/// We cannot use assetBundle.LoadAssetAsync to load a scene, so a dummy method is needed to pass the scene up the stream
		/// </summary>
		/// <param name="obj">Scene</param>
		/// <param name="observer">Observer</param>
		/// <param name="cancellationToken">Cancellation Token</param>
		/// <returns></returns>
		private IEnumerator RunSceneRequestOperation<T>(T obj, IObserver<T> observer, CancellationToken cancellationToken)where T : UnityEngine.Object
		{
			yield return null;

			observer.OnNext(obj);
			observer.OnCompleted();
		}

		private IEnumerator RunAssetBundleRequestOperation<T>(UnityEngine.AssetBundleRequest asyncOperation, IObserver<T> observer, CancellationToken cancellationToken)where T : UnityEngine.Object
		{
			while (!asyncOperation.isDone && !cancellationToken.IsCancellationRequested)
				yield return null;

			if (!cancellationToken.IsCancellationRequested)
			{
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
}