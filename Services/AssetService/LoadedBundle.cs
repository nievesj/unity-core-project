using System;
using System.Collections;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Core.Services.Assets
{
    /// <summary>
    /// Represents a loaded Asset bundle and the operations that can be applied to it
    /// </summary>
    public class LoadedBundle
    {
        private AssetBundle _assetBundle;
        private ManifestInfo _manifestInfo;
        internal AssetBundle Bundle => _assetBundle;

        public LoadedBundle(AssetBundle asset)
        {
            _assetBundle = asset;
        }

        public LoadedBundle(AssetBundle asset, ManifestInfo info)
        {
            _assetBundle = asset;
            _manifestInfo = info;
        }

        /// <summary>
        /// Unload bundle. Only use this when the bundle is no longer needed.
        /// </summary>
        /// <param name="unloadAll"></param>
        public void Unload(bool unloadAll = false)
        {
            if (_assetBundle)
                _assetBundle.Unload(unloadAll);
        }

        /// <summary>
        /// Loads an asset inside this bundle
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
        {
            //TODO: Change this to GetComponentFromAssetBundle
            Debug.Log(("LoadedBundle: Asynchronously loading asset: " + name).Colored(Colors.Yellow));

            //If the asset bundle contains a scene, pass the scene up the stream so it can be loaded
            // if (_assetBundle.GetAllScenePaths().Length > 0)
            //     return Observable.FromCoroutine<T>((observer) => RunSceneRequestOperation<T>(_assetBundle as T, observer));
            // else
            
                return await RunAssetBundleRequestAsync<T>(_assetBundle.LoadAssetAsync(name));
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
            //TODO: improve scene loading
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
        private async Task<T> RunAssetBundleRequestAsync<T>(UnityEngine.AssetBundleRequest asyncOperation) where T : UnityEngine.Object
        {
            await asyncOperation;

            if (!asyncOperation.asset)
                throw new Exception("RunAssetBundleRequestOperation: Error getting bundle.");

            //Current use case returns the component, if this changes then deal with it downstream but for now this should be ok
            var go = asyncOperation.asset as GameObject;
            var comp = go.GetComponent<T>();

            return comp;
        }
    }
}