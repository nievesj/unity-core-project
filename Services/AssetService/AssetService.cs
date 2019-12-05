using System;
using System.Collections.Generic;
using System.Threading;
using UniRx.Async;
using UnityEngine;
using Zenject;
using Logger = UnityLogger.Logger;
using LogType = UnityLogger.LogType;

namespace Core.Services.Assets
{
    /// <summary>
    /// Central point to get asset bundles required to run the game.
    /// </summary>
    public class AssetService : Service
    {
        public bool UseStreamingAssets => Configuration.UseStreamingAssets;

        public AssetCacheState AssetCacheState => Configuration.UseCache ? AssetCacheState.Cache : AssetCacheState.NoCache;

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
                Logger.Log(("---- AssetService: Unity Cloud Build Manifest present. Build Version: " + cloudManifest.buildNumber),Colors.Aqua);
                _cloudBuildManifest = cloudManifest;
            }
            else
            {
                Logger.Log("---- AssetService: Unity Cloud Build Manifest missing. This is ok. Ignoring.",Colors.Aqua);
            }
        }

        private async UniTask<T> LoadAsset<T>(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets = false,
            IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken)) where T : UnityEngine.Object
        {
            return await _assetBundlebundleLoader.LoadAsset<T>(bundleRequest, forceLoadFromStreamingAssets, progress, cancellationToken);
        }

        /// <summary>
        /// Gets and loads the required asset bundle
        /// </summary>
        /// <param name="assetCatRoot"></param>
        /// <param name="assetName">Bundle name and asset name are the same</param>
        /// <param name="forceLoadFromStreamingAssets">
        /// Forces loading from StreamingAssets folder. Useful for when including assets
        /// with the build
        /// </param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> Observable </returns>
        public async UniTask<T> LoadAsset<T>(AssetCategoryRoot assetCatRoot, string assetName, bool forceLoadFromStreamingAssets = false,
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
        /// <param name="forceLoadFromStreamingAssets">
        /// Forces loading from StreamingAssets folder. Useful for when including assets
        /// with the build
        /// </param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> Observable </returns>
        public async UniTask<T> LoadAsset<T>(AssetCategoryRoot assetCatRoot, string bundleName, string assetName,
            bool forceLoadFromStreamingAssets = false, IProgress<float> progress = null,
            CancellationToken cancellationToken = default(CancellationToken)) where T : UnityEngine.Object
        {
            var bundleNeeded = new BundleRequest(assetCatRoot,
                bundleName, assetName);

            return await LoadAsset<T>(bundleNeeded, forceLoadFromStreamingAssets, progress, cancellationToken);
        }

        public async UniTask<UnityEngine.Object> GetScene(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets = false,
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
        public async UniTask<Dictionary<string, LoadedBundle>> LoadMultipleBundles(List<BundleRequest> requests,
            bool forceLoadFromStreamingAssets = false, IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tempCount = 0;
            var total = requests.Count;
            var bundles = new Dictionary<string, LoadedBundle>();

            foreach (var request in requests)
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                var bundle = await _assetBundlebundleLoader.LoadBundle(request, forceLoadFromStreamingAssets, cancellationToken: cancellationToken);
                bundles.Add(request.BundleName, bundle);

                tempCount++;
                progress?.Report(tempCount * 100 / total);

                Logger.Log($"LoadMultipleAssets: {tempCount} of {total} | {tempCount * 100 / total}%",Colors.Aquamarine);
            }

            return bundles;
        }

        /// <summary>
        /// Unloads asset and removes it from memory
        /// </summary>
        /// <param name="name">Asset name </param>
        /// <param name="unloadAllDependencies"> Unload all dependencies? </param>
        public async UniTask UnloadAsset(string name, bool unloadAllDependencies)
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