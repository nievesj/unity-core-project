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
        internal async Task<T> LoadAsset<T>(BundleRequest bundleRequest) where T : Object
        {
#if UNITY_EDITOR
            if (EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES)
                return await SimulateAssetBundle<T>(bundleRequest);
#endif
            var bundle = await LoadBundle(bundleRequest);
            return await bundle.LoadAssetAsync<T>(bundleRequest.AssetName);
        }

        internal async Task<LoadedBundle> LoadBundle(BundleRequest bundleRequest)
        {
            if (!_loadedBundles.ContainsKey(bundleRequest.BundleName))
            {
                AssetBundle bundle;
                if (!_assetService.UseStreamingAssets)
                    bundle = await GetBundleFromWebOrCacheAsync(bundleRequest);
                else
                    bundle = await GetBundleFromStreamingAssetsAsync(bundleRequest);
                
                _loadedBundles.Add(bundleRequest.BundleName, new LoadedBundle(bundle));
            }

            return _loadedBundles[bundleRequest.BundleName];
        }

        internal async Task<Object> LoadScene(BundleRequest bundleRequest)
        {
            var bundle = await LoadBundle(bundleRequest);

            if (bundle.Bundle.GetAllScenePaths().Length > 0)
                return bundle.Bundle;

            return null;
        }

        /// <summary>
        /// Unloads asset and removes it from memory. Only do this when the asset is no longer needed.
        /// </summary>
        /// <param name="name">                  Asset name </param>
        /// <param name="unloadAllDependencies"> Unload all dependencies? </param>
        internal void UnloadAssetBundle(string name, bool unloadAllDependencies)
        {
            name = name.ToLower();

            if (_loadedBundles.ContainsKey(name))
            {
                _loadedBundles[name].Unload(unloadAllDependencies);
                _loadedBundles.Remove(name);

                Resources.UnloadUnusedAssets();
            }
        }

        internal T GetLoadedBundle<T>(string name) where T : Object
        {
            if (_loadedBundles.ContainsKey(name.ToLower()))
                return _loadedBundles[name.ToLower()].Bundle as T;
            
             return null;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Method attemps to get an asset from the asset database.
        /// </summary>
        /// <param name="bundleRequest">     Bundle to request </param>
        /// <returns> IEnumerator </returns>
        private async Task<T> SimulateAssetBundle<T>(BundleRequest bundleRequest) where T : Object
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
        private async Task<AssetBundle> GetBundleFromWebOrCacheAsync(BundleRequest bundleRequest)
        {
            var www = new UnityWebRequest();
            using (www)
            {
                Debug.Log(("AssetBundleLoader: " + _assetService.AssetCacheState + " | Requesting: " + bundleRequest.AssetName + " | " + bundleRequest.BundleName).Colored(Colors.Aqua));
                if (_assetService.AssetCacheState == AssetCacheState.Cache && _assetService.AssetCacheStrategy == AssetCacheStrategy.UseUnityCloudManifestBuildVersion)
                {
                    //cache bundles by using Unity Cloud Build manifest
                    uint buildNumber = 0;
                    if (_assetService.CloudBuildManifest != null)
                        buildNumber = System.Convert.ToUInt32(_assetService.CloudBuildManifest.buildNumber);

                    www = UnityWebRequestAssetBundle.GetAssetBundle(bundleRequest.AssetPath, buildNumber, 0);
                }
                else if (_assetService.AssetCacheState == AssetCacheState.NoCache)
                {
                    //No caching, just get the bundle
                    www = UnityWebRequestAssetBundle.GetAssetBundle(bundleRequest.AssetPath);
                }

                //Wait until www is done.
                await www.SendWebRequest();

                //get bundle
                var bundle = DownloadHandlerAssetBundle.GetContent(www);

                if (www.isNetworkError)
                    throw new System.Exception("AssetBundleLoader: " + www.error);

                return bundle;
            }
        }

        /// <summary>
        /// Gets bundle from streaming assets directory
        /// </summary>
        /// <param name="bundleRequest"> Bundle to request </param>
        /// <returns> Observable </returns>
        private async Task<AssetBundle> GetBundleFromStreamingAssetsAsync(BundleRequest bundleRequest) 
        {
            Debug.Log(("AssetBundleLoader: Using StreamingAssets - " + " Requesting:" + bundleRequest.AssetCategory + " | " + bundleRequest.BundleName).Colored(Colors.Aqua));
            var path = Path.Combine(Application.streamingAssetsPath, bundleRequest.AssetPathFromLocalStreamingAssets);

            return await AssetBundle.LoadFromFileAsync(path);
        }
    }
}