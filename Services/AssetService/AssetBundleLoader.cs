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
        /// <param name="forceLoadFromStreamingAssets">Forces loading from StreamingAssets folder. Useful for when including assets with the build</param>
        /// <returns> Observable </returns>
        internal async Task<T> LoadAsset<T>(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets) where T : Object
        {
            var bundle = await LoadBundle(bundleRequest, forceLoadFromStreamingAssets);
            return await bundle.LoadAssetAsync<T>(bundleRequest.AssetName);
        }

        internal async Task<LoadedBundle> LoadBundle(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets)
        {
            if (!_loadedBundles.ContainsKey(bundleRequest.BundleName))
            {
                
#if UNITY_EDITOR
                if (EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES)
                {
                    var assPath = bundleRequest.AssetCategory.ToString().ToLower() + "/" + bundleRequest.BundleName;
                    var paths = AssetDatabase.GetAssetPathsFromAssetBundle(assPath);

                    var theone = string.Empty;
                    foreach (var path in paths)
                    {
                        if (path.ToLower().Contains(bundleRequest.AssetName))
                        {
                            theone = path;
                            break;
                        }
                    }

                    var asset = AssetDatabase.LoadAssetAtPath<GameObject>(theone);
                    _loadedBundles.Add(bundleRequest.BundleName, new LoadedBundle(asset));

                    return _loadedBundles[bundleRequest.BundleName];
                }
#endif
                
                AssetBundle bundle;
                if (_assetService.UseStreamingAssets || forceLoadFromStreamingAssets)
                    bundle = await GetBundleFromStreamingAssetsAsync(bundleRequest);
                else
                    bundle = await GetBundleFromWebOrCacheAsync(bundleRequest);

                _loadedBundles.Add(bundleRequest.BundleName, new LoadedBundle(bundle));
            }

            return _loadedBundles[bundleRequest.BundleName];
        }

        internal async Task<Object> LoadScene(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets = false)
        {
            var bundle = await LoadBundle(bundleRequest, forceLoadFromStreamingAssets);
            return bundle.Bundle.GetAllScenePaths().Length > 0 ? bundle.Bundle : null;
        }

        /// <summary>
        /// Unloads asset and removes it from memory. Only do this when the asset is no longer needed.
        /// </summary>
        /// <param name="name"> Asset name </param>
        /// <param name="unloadAllDependencies"> Unload all dependencies? </param>
        internal async Task UnloadAssetBundle(string name, bool unloadAllDependencies)
        {
            name = name.ToLower();
            
            if (_loadedBundles.ContainsKey(name))
            {
                _loadedBundles[name].Unload(unloadAllDependencies);
                _loadedBundles.Remove(name);

                await Resources.UnloadUnusedAssets();
            }
        }

        internal AssetBundle GetLoadedBundle(string name)
        {
            if (_loadedBundles.ContainsKey(name.ToLower()))
                return _loadedBundles[name.ToLower()].Bundle;

            return null;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Method attemps to get an asset from the asset database.
        /// </summary>
        /// <param name="bundleRequest"> Bundle to request </param>
        private async Task<T> SimulateAssetBundle<T>(BundleRequest bundleRequest) where T : Object
        {
            Debug.Log($"AssetBundleLoader: Simulated | Requesting: {bundleRequest.AssetName}  {bundleRequest.BundleName}".Colored(Colors.Aqua));

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
                if (asset && asset.name.ToLower() == bundleRequest.BundleName.ToLower())
                {
                    //Stop loop when the first asset of T is found
                    assets.Add(asset);
                    break;
                }

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
        private async Task<AssetBundle> GetBundleFromWebOrCacheAsync(BundleRequest bundleRequest)
        {
            var uwr = new UnityWebRequest();

            Debug.Log($"AssetBundleLoader:  {_assetService.AssetCacheState}  | Requesting:  {bundleRequest.AssetName}  {bundleRequest.BundleName}".Colored(Colors.Aqua));
            if (_assetService.CloudBuildManifest != null && _assetService.AssetCacheState == AssetCacheState.Cache && _assetService.AssetCacheStrategy == AssetCacheStrategy.UseUnityCloudManifestBuildVersion)
            {
                //cache bundles by using Unity Cloud Build manifest
                uint buildNumber = 0;
                buildNumber = System.Convert.ToUInt32(_assetService.CloudBuildManifest.buildNumber);
                uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleRequest.GetAssetPath(_assetService.Configuration), buildNumber, 0);
            }
            else if (_assetService.CloudBuildManifest == null || _assetService.AssetCacheState == AssetCacheState.NoCache)
            {
                if (_assetService.AssetCacheState == AssetCacheState.Cache)
                    Debug.Log("AssetBundleLoader:  Caching is enabled, but Unity Cloud Build Manifest was missing, bundle was not cached.".Colored(Colors.Aqua));

                //No caching, just get the bundle
                uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleRequest.GetAssetPath(_assetService.Configuration));
            }

            //Wait until www is done.
            await uwr.SendWebRequest();

            //get bundle
            var bundle = DownloadHandlerAssetBundle.GetContent(uwr);

            if (uwr.isNetworkError)
                throw new System.Exception($"AssetBundleLoader:  {uwr.error}");

            uwr.Dispose();
            
            return bundle;
        }

        /// <summary>
        /// Gets bundle from streaming assets directory
        /// </summary>
        /// <param name="bundleRequest"> Bundle to request </param>
        private async Task<AssetBundle> GetBundleFromStreamingAssetsAsync(BundleRequest bundleRequest)
        {
            Debug.Log($"AssetBundleLoader: Using StreamingAssets -  Requesting: {bundleRequest.AssetCategory}  {bundleRequest.BundleName}".Colored(Colors.Aqua));
            var path = Path.Combine(Application.streamingAssetsPath, bundleRequest.AssetPathFromLocalStreamingAssets);
           
            return await AssetBundle.LoadFromFileAsync(path);
        }
    }
}