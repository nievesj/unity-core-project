using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        [Inject]
        private AssetService _assetService;

        //Keeps track of the bundles that have been loaded
        private Dictionary<string, LoadedBundle> _loadedBundles;

        /// <summary>
        /// Initialize object
        /// </summary>
        internal AssetBundleLoader()
        {
            _loadedBundles = new Dictionary<string, LoadedBundle>();
        }

        /// <summary>
        /// Attemps to load requested asset. Depending on the project options it will look the asset
        /// on the web, on the streaming assets folder or it will attempt to simulate it by loading
        /// it from the asset database. Asset simulation is only available on editor.
        /// </summary>
        /// <param name="bundleRequest"> Bundle to request </param>
        /// <returns> Observable </returns>
        internal async Task<T> LoadAsset<T>(BundleRequest bundleRequest) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES)
                return await SimulateAssetBundle<T>(bundleRequest);
#endif
            if (!_assetService.UseStreamingAssets)
                return await GetBundleFromWebOrCacheOperation<T>(bundleRequest);
            else
                return await GetBundleFromStreamingAssets<T>(bundleRequest);
        }

        /// <summary>
        /// Unloads asset and removes it from memory. Only do this when the asset is no longer needed.
        /// </summary>
        /// <param name="name">                  Asset name </param>
        /// <param name="unloadAllDependencies"> Unload all dependencies? </param>
        internal void UnloadAsset(string name, bool unloadAllDependencies)
        {
            name = name.ToLower();

            if (_loadedBundles.ContainsKey(name))
            {
                _loadedBundles[name].Unload(unloadAllDependencies);
                _loadedBundles.Remove(name);

                Resources.UnloadUnusedAssets();
            }
        }

        internal T GetLoadedBundle<T>(string name) where T : UnityEngine.Object
        {
            if (_loadedBundles.ContainsKey(name.ToLower()))
                return _loadedBundles[name.ToLower()].Bundle as T;
            else return null;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Method attemps to get an asset from the asset database.
        /// </summary>
        /// <param name="bundleRequest">     Bundle to request </param>
        /// <returns> IEnumerator </returns>
        private async Task<T> SimulateAssetBundle<T>(BundleRequest bundleRequest) where T : UnityEngine.Object
        {
            Debug.Log(("AssetBundleLoader: Simulated | Requesting: " + bundleRequest.AssetName + " | " + bundleRequest.BundleName).Colored(Colors.Aqua));

            var assets = new List<T>();
            //Get guid from the asset
            var guids = AssetDatabase.FindAssets(bundleRequest.BundleName);

            //This will give you a bunch of assets of the same name, we need to filter it further
            foreach (var id in guids)
            {
                //Get path
                var path = AssetDatabase.GUIDToAssetPath(id);
                //Get actual asset on that path of the type we're looking for.
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                //This assumes that assets of the same type are located in the same directory, thus, they should have unique names.
                if (asset && asset.name.ToLower().Equals(bundleRequest.BundleName.ToLower()))
                {
                    //Stop loop when the first asset of T is found
                    assets.Add(asset);
                    break;
                }

                //Not really needed but I want to keep the async pattern
                await new WaitForEndOfFrame();
            }

            //Show error if there are duplicate assets on different locations. This should be avoided...
            if (assets.Count > 1)
                Debug.LogError("Duplicate asset names. You have two assets of the same name in different locations. Try using unique names for your asset bundles.");

            return assets.First();
        }

#endif

        /// <summary>
        /// Method attemps to get a bundle from the web/cloud
        /// </summary>
        /// <param name="bundleRequest">     Bundle to request </param>
        /// <returns> IEnumerator </returns>
        private async Task<T> GetBundleFromWebOrCacheOperation<T>(BundleRequest bundleRequest) where T : UnityEngine.Object
        {
            UnityWebRequest www = null;
            var manifestInfo = new ManifestInfo(bundleRequest, _assetService.Configuration.ManifestCachePeriod);

            Debug.Log(("AssetBundleLoader: " + _assetService.AssetCacheState + " | Requesting: " + bundleRequest.AssetName + " | " + bundleRequest.BundleName).Colored(Colors.Aqua));

            if (_assetService.AssetCacheState.Equals(AssetCacheState.Cache) && _assetService.AssetCacheStrategy.Equals(AssetCacheStrategy.UseUnityCloudManifestBuildVersion))
            {
                //cache bundles by using Unity Cloud Build manifest
                uint buildNumber = 0;
                if (_assetService.CloudBuildManifest != null)
                    buildNumber = System.Convert.ToUInt32(_assetService.CloudBuildManifest.buildNumber);

                www = UnityWebRequestAssetBundle.GetAssetBundle(bundleRequest.AssetPath, buildNumber, 0);
            }
            else if (_assetService.AssetCacheState.Equals(AssetCacheState.NoCache))
            {
                //No caching, just get the bundle
                www = UnityWebRequestAssetBundle.GetAssetBundle(bundleRequest.AssetPath);
            }

            //Wait until www is done.
            await www.SendWebRequest();

            //get bundle
            var bundle = DownloadHandlerAssetBundle.GetContent(www);
            www.Dispose(); //TODO: use using

            if (www.isNetworkError)
            {
                throw new System.Exception("AssetBundleLoader: " + www.error);
            }
            else
            {
                //Extract asset from bundle. This assumes that theres is only one asset per bundle. Other use cases will need to modify this.
                return await ProcessDownloadedBundle<T>(bundleRequest, new LoadedBundle(bundle, manifestInfo));
            }
        }

        /// <summary>
        /// Gets bundle from streaming assets directory
        /// </summary>
        /// <param name="bundleRequest"> Bundle to request </param>
        /// <returns> Observable </returns>
        private async Task<T> GetBundleFromStreamingAssets<T>(BundleRequest bundleRequest) where T : UnityEngine.Object
        {
            Debug.Log(("AssetBundleLoader: Using StreamingAssets - " + " Requesting:" + bundleRequest.AssetCategory + " | " + bundleRequest.BundleName).Colored(Colors.Aqua));
            var path = Path.Combine(Application.streamingAssetsPath, bundleRequest.AssetPathFromLocalStreamingAssets);

            return await RunAssetBundleCreateRequestOperation<T>(path, bundleRequest);
        }

        /// <summary>
        /// Operation to get bundle from streaming assets directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bundleRequest">Bundle to request </param>
        /// <returns></returns>
        private async Task<T> RunAssetBundleCreateRequestOperation<T>(string path, BundleRequest bundleRequest) where T : UnityEngine.Object
        {
            var assetBundle = await AssetBundle.LoadFromFileAsync(path);
            return await ProcessDownloadedBundle<T>(bundleRequest, new LoadedBundle(assetBundle));
        }

        /// <summary>
        /// Extracts required asset from the asset bundle
        /// </summary>
        /// <param name="bundleRequest"> Bundle to request </param>
        /// <param name="bundle">        Bundle </param>
        /// <returns></returns>
        private async Task<T> ProcessDownloadedBundle<T>(BundleRequest bundleRequest, LoadedBundle bundle) where T : UnityEngine.Object
        {
            if (!_loadedBundles.ContainsKey(bundleRequest.BundleName))
                _loadedBundles.Add(bundleRequest.BundleName, bundle);

            return await bundle.LoadAssetAsync<T>(bundleRequest.AssetName);
        }
    }
}