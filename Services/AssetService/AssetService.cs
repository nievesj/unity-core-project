using System;
using System.Collections.Generic;
using System.Threading;
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

        private async Task<T> LoadAsset<T>(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets = false,
            IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken)) where T : UnityEngine.Object
        {
            return await _assetBundlebundleLoader.LoadAsset<T>(bundleRequest, forceLoadFromStreamingAssets, progress, cancellationToken);
        }

        /// <summary>
        /// Gets and loads the required asset bundle
        /// </summary>
        /// <param name="assetCatRoot"></param>
        /// <param name="assetName">Bundle name and asset name are the same</param>
        /// <param name="forceLoadFromStreamingAssets">Forces loading from StreamingAssets folder. Useful for when including assets with the build</param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> Observable </returns>
        public async Task<T> LoadAsset<T>(AssetCategoryRoot assetCatRoot, string assetName, bool forceLoadFromStreamingAssets = false,
            IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken)) where T : UnityEngine.Object
        {
            var bundleNeeded = new BundleRequest(assetCatRoot,
                assetName, assetName);

            return await LoadAsset<T>(bundleNeeded, forceLoadFromStreamingAssets, progress, cancellationToken);
        }

        /// <summary>
        /// Gets and loads the required asset bundle
        /// </summary>
        /// <param name="assetCatRoot"></param>
        /// <param name="bundleName"></param>
        /// <param name="assetName">Bundle name and asset name are the same</param>
        /// <param name="forceLoadFromStreamingAssets">Forces loading from StreamingAssets folder. Useful for when including assets with the build</param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> Observable </returns>
        public async Task<T> LoadAsset<T>(AssetCategoryRoot assetCatRoot, string bundleName, string assetName,
            bool forceLoadFromStreamingAssets = false, IProgress<float> progress = null,
            CancellationToken cancellationToken = default(CancellationToken)) where T : UnityEngine.Object
        {
            var bundleNeeded = new BundleRequest(assetCatRoot,
                bundleName, assetName);

            return await LoadAsset<T>(bundleNeeded, forceLoadFromStreamingAssets, progress, cancellationToken);
        }

        public async Task<UnityEngine.Object> GetScene(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets = false,
            IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _assetBundlebundleLoader.LoadScene(bundleRequest, forceLoadFromStreamingAssets, progress, cancellationToken);
        }

        /// <summary>
        /// Utility method to request multiple assets. 
        /// </summary>
        /// <param name="requests">Bundle requests</param>
        /// <param name="progress">Reports loading progress percentage</param>
        /// <param name="forceLoadFromStreamingAssets"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, LoadedBundle>> LoadMultipleBundles(List<BundleRequest> requests,
            bool forceLoadFromStreamingAssets = false, IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var total = requests.Count;
            var bundles = new Dictionary<string, LoadedBundle>();
            var percent = 0;

            var process = await Task.Run(async () =>
            {
                var tempCount = 0;
                foreach (var request in requests)
                {
                    if(cancellationToken.IsCancellationRequested)
                        return 0;
                    
                    await Awaiters.WaitForUpdate; //Wait for main thread

                    var bundle = await _assetBundlebundleLoader.LoadBundle(request, forceLoadFromStreamingAssets, cancellationToken: cancellationToken);
                    bundles.Add(request.BundleName, bundle);

                    tempCount++;
                    percent = tempCount * 100 / total;
                    progress?.Report(percent);

                    Debug.Log($"LoadMultipleAssets: {tempCount} of {total} | {percent}%".Colored(Colors.Aquamarine));
                }

                return tempCount;
            }, cancellationToken);

            return bundles;
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
        public AssetBundle GetLoadedBundle(string name)
        {
            return _assetBundlebundleLoader.GetLoadedBundle(name);
        }
    }
}