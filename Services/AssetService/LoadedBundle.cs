using System;
using System.Collections;
using System.Threading.Tasks;
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
            Debug.Log(("LoadedBundle: Async loading asset: " + name).Colored(Colors.Yellow));

            return await GetAssetCompomnentAsync<T>(_assetBundle.LoadAssetAsync(name));
        }

        /// <summary>
        /// Operation extracts an asset from the loaded bundle
        /// </summary>
        /// <param name="asyncOperation">   </param>
        /// <returns></returns>
        private async Task<T> GetAssetCompomnentAsync<T>(AssetBundleRequest asyncOperation) where T : UnityEngine.Object
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