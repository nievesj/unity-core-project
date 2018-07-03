using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core.Services.Assets
{
    /// <summary>
    /// Central point to get asset bundles required to run the game.
    /// </summary>
    public class AssetService : Service
    {
        public bool UseStreamingAssets => Configuration.UseStreamingAssets;

        public AssetCacheState AssetCacheState => Configuration.UseCache ? AssetCacheState.Cache : AssetCacheState.NoCache;

        public AssetCacheStrategy AssetCacheStrategy => AssetCacheStrategy.UseUnityCloudManifestBuildVersion;

        private UnityCloudBuildManifest _cloudBuildManifest;

        public UnityCloudBuildManifest CloudBuildManifest => _cloudBuildManifest;

        public readonly AssetServiceConfiguration Configuration;

        [Inject]
        private AssetBundleLoader _assetBundlebundleLoader;

        public AssetService(ServiceConfiguration config)
        {
            Configuration = config as AssetServiceConfiguration;
            LoadBuildManifestAsync();
        }

        private async void LoadBuildManifestAsync()
        { 
            var cloudManifest = await UnityCloufBuildManifestLoader.LoadBuildManifest();
            if (cloudManifest != null)
            {
                Debug.Log(("---- AssetService: Unity Cloud Build Manifest present. Build Version: " + cloudManifest.buildNumber).Colored(Colors.Aqua));
                _cloudBuildManifest = cloudManifest;
            }
            else
            {
                Debug.Log("---- AssetService: Unity Cloud Build Manifest missing. This is ok. Ignoring.".Colored(Colors.Aqua));
            }
        }

        private async Task<T> GetAndLoadAsset<T>(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets = false) where T : UnityEngine.Object
        {
            return await _assetBundlebundleLoader.LoadAsset<T>(bundleRequest, forceLoadFromStreamingAssets);
        }

        /// <summary>
        /// Gets and loads the required asset bundle
        /// </summary>
        /// <param name="assetCatRoot"></param>
        /// <param name="assetName">Bundle name and asset name are the same</param>
        /// <param name="forceLoadFromStreamingAssets">Forces loading from StreamingAssets folder. Useful for when including assets with the build</param>
        /// <returns> Observable </returns>
        public async Task<T> GetAndLoadAsset<T>(AssetCategoryRoot assetCatRoot, string assetName, bool forceLoadFromStreamingAssets = false) where T : UnityEngine.Object
        {
            var bundleNeeded = new BundleRequest(assetCatRoot, 
                assetName, assetName, Configuration);

            return await GetAndLoadAsset<T>(bundleNeeded, forceLoadFromStreamingAssets);
        }

        /// <summary>
        /// Gets and loads the required asset bundle
        /// </summary>
        /// <param name="assetCatRoot"></param>
        /// <param name="bundleName"></param>
        /// <param name="assetName">Bundle name and asset name are the same</param>
        /// <param name="forceLoadFromStreamingAssets">Forces loading from StreamingAssets folder. Useful for when including assets with the build</param>
        /// <returns> Observable </returns>
        public async Task<T> GetAndLoadAsset<T>(AssetCategoryRoot assetCatRoot, string bundleName, string assetName, bool forceLoadFromStreamingAssets = false) where T : UnityEngine.Object
        {
            var bundleNeeded = new BundleRequest(assetCatRoot, 
                bundleName, assetName, Configuration);

            return await GetAndLoadAsset<T>(bundleNeeded);
        }

        public async Task<UnityEngine.Object> GetScene(BundleRequest bundleRequest) 
        {
            return await _assetBundlebundleLoader.LoadScene(bundleRequest);
        }

        /// <summary>
        /// Unloads asset and removes it from memory
        /// </summary>
        /// <param name="name">Asset name </param>
        /// <param name="unloadAllDependencies"> Unload all dependencies? </param>
        public async Task UnloadAsset(string name, bool unloadAllDependencies)
        {
            await _assetBundlebundleLoader.UnloadAssetBundle(name, unloadAllDependencies);
        }

        /// <summary>
        /// Gets a bundle that has been previously loaded and it's stored in memory.
        /// </summary>
        /// <param name="name"> Asset name </param>
        /// <returns> T </returns>
        public T GetLoadedBundle<T>(string name) where T : UnityEngine.Object
        {
            return _assetBundlebundleLoader.GetLoadedBundle<T>(name);
        }
    }
}