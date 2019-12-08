using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Core.Common.Extensions.String;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
using Logger = UnityLogger.Logger;
using Object = UnityEngine.Object;
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
        /// <param name="forceLoadFromStreamingAssets">
        /// Forces loading from StreamingAssets folder. Useful for when including assets
        /// with the build
        /// </param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> Observable </returns>
        internal async UniTask<T> LoadAsset<T>(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets,
            IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken)) where T : UnityEngine.Object
        {
            var bundle = await LoadBundle(bundleRequest, forceLoadFromStreamingAssets, progress, cancellationToken);
            return await bundle.LoadAssetAsync<T>(bundleRequest.AssetName, progress, cancellationToken);
        }

        internal async UniTask<LoadedBundle> LoadBundle(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets,
            IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_loadedBundles.ContainsKey(bundleRequest.BundleName))
            {
#if UNITY_EDITOR
                if (EditorPreferences.EditorprefSimulateAssetBundles)
                {
                    var assPath = bundleRequest.AssetCategory.ToString().ToLower() + "/" + bundleRequest.BundleName;
                    var paths = AssetDatabase.GetAssetPathsFromAssetBundle(assPath);

                    var theone = string.Empty;
                    foreach (var path in paths)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return null;

                        if (path.ToLower().Contains(bundleRequest.AssetName))
                        {
                            theone = path;
                            break;
                        }
                    }

                    Object asset;

                    if (bundleRequest.AssetCategory == AssetCategoryRoot.Scenes)
                    {
                        asset = AssetDatabase.LoadAssetAtPath<Object>(theone);
                        _loadedBundles.Add(bundleRequest.BundleName, new LoadedBundle(asset as SceneAsset));
                    }
                    else
                    {
                        asset = AssetDatabase.LoadAssetAtPath<Object>(theone);
                        _loadedBundles.Add(bundleRequest.BundleName, new LoadedBundle(asset as GameObject));
                    }

                    return _loadedBundles[bundleRequest.BundleName];
                }
#endif

                AssetBundle bundle;
                if (_assetService.UseStreamingAssets || forceLoadFromStreamingAssets)
                    bundle = await GetBundleFromStreamingAssetsAsync(bundleRequest, progress, cancellationToken);
                else
                    bundle = await GetBundleFromWebOrCacheAsync(bundleRequest, progress, cancellationToken);

                _loadedBundles.Add(bundleRequest.BundleName, new LoadedBundle(bundle));
            }

            return _loadedBundles[bundleRequest.BundleName];
        }

        internal async UniTask<UnityEngine.Object> LoadScene(BundleRequest bundleRequest, bool forceLoadFromStreamingAssets = false,
            IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var bundle = await LoadBundle(bundleRequest, forceLoadFromStreamingAssets, progress, cancellationToken);

#if UNITY_EDITOR
            if (EditorPreferences.EditorprefSimulateAssetBundles)
            {
                return bundle.SceneAsset;
            }
#endif
            if (bundle.Bundle.GetAllScenePaths().Length == 0)
                throw new Exception($"AssetBundleLoader: Tried to load a scene from {bundleRequest.BundleName}, and this bundle does not contains a scene.");

            return bundle.Bundle; //.GetAllScenePaths().Length > 0 ? bundle.Bundle : null;
        }

        /// <summary>
        /// Unloads asset and removes it from memory. Only do this when the asset is no longer needed.
        /// </summary>
        /// <param name="name"> Asset name </param>
        /// <param name="unloadAllDependencies"> Unload all dependencies? </param>
        internal async UniTask UnloadAssetBundle(string name, bool unloadAllDependencies)
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
            return _loadedBundles.ContainsKey(name.ToLower()) ? _loadedBundles[name.ToLower()].Bundle : null;
        }

        /// <summary>
        /// Method attemps to get a bundle from the web/cloud
        /// </summary>
        /// <param name="bundleRequest">Bundle to request</param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        private async UniTask<AssetBundle> GetBundleFromWebOrCacheAsync(BundleRequest bundleRequest,
            IProgress<float> progress, CancellationToken cancellationToken)
        {
            var uwr = new UnityWebRequest();

            Logger.Log($"AssetBundleLoader:  {_assetService.AssetCacheState}  | Requesting:  {bundleRequest.AssetName}  {bundleRequest.BundleName}",Colors.Aqua);
            if (_assetService.CloudBuildManifest != null && _assetService.AssetCacheState == AssetCacheState.Cache)
            {
                //cache bundles by using Unity Cloud Build manifest
                uint buildNumber = 0;
                buildNumber = Convert.ToUInt32(_assetService.CloudBuildManifest.buildNumber);
                uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleRequest.GetAssetPath(_assetService.Configuration), buildNumber, 0);
            }
            else if (_assetService.CloudBuildManifest == null || _assetService.AssetCacheState == AssetCacheState.NoCache)
            {
                if (_assetService.AssetCacheState == AssetCacheState.Cache)
                    Logger.Log("AssetBundleLoader:  Caching is enabled, but Unity Cloud Build Manifest was missing, bundle was not cached.",Colors.Aqua);

                //No caching, just get the bundle
                uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleRequest.GetAssetPath(_assetService.Configuration));
            }

            //Wait until uwr is done.
            var asyncOperation = uwr.SendWebRequest();
            while (!asyncOperation.isDone)
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                progress?.Report(asyncOperation.progress);
                Logger.Log($"GetBundleFromWebOrCacheAsync {bundleRequest.BundleName} progress: {asyncOperation.progress * 100f}%",Colors.LightSalmon);
            }

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
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        private async UniTask<AssetBundle> GetBundleFromStreamingAssetsAsync(BundleRequest bundleRequest,
            IProgress<float> progress, CancellationToken cancellationToken)
        {
            Logger.Log($"AssetBundleLoader: Using StreamingAssets -  Requesting: {bundleRequest.AssetCategory}  {bundleRequest.BundleName}",Colors.Aqua);
            var path = Path.Combine(Application.streamingAssetsPath, bundleRequest.AssetPathFromLocalStreamingAssets);

            var asyncOperation = AssetBundle.LoadFromFileAsync(path);
            while (!asyncOperation.isDone)
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                progress?.Report(asyncOperation.progress);
                Logger.Log($"GetBundleFromStreamingAssetsAsync {bundleRequest.BundleName} progress: {asyncOperation.progress * 100f}%",Colors.LightSalmon);
            }

            return asyncOperation.assetBundle;
        }
    }
}