﻿using System;
using System.Threading;
using UniRx.Async;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


namespace Core.Services.Assets
{
    /// <summary>
    /// Represents a loaded Asset bundle and the operations that can be applied to it
    /// </summary>
    public class LoadedBundle
    {
        private readonly GameObject _simulatedAsset;
        private readonly UnityEngine.Object _simulatedAssetObject;

        internal AssetBundle Bundle { get; }

#if UNITY_EDITOR
        internal SceneAsset SceneAsset { get; }
#endif

        public LoadedBundle(AssetBundle asset)
        {
            Bundle = asset;
        }

        public LoadedBundle(GameObject asset)
        {
            _simulatedAsset = asset;
        }

#if UNITY_EDITOR
        public LoadedBundle(SceneAsset asset)
        {
            SceneAsset = asset;
        }
#endif

        /// <summary>
        /// Unload bundle. Only use this when the bundle is no longer needed.
        /// </summary>
        /// <param name="unloadAll"></param>
        public void Unload(bool unloadAll = false)
        {
            if (Bundle)
                Bundle.Unload(unloadAll);
        }

        /// <summary>
        /// Loads an asset inside this bundle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async UniTask<T> LoadAssetAsync<T>(string name, IProgress<float> progress,
            CancellationToken cancellationToken) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (EditorPreferences.EditorprefSimulateAssetBundles)
            {
                Debug.Log(("LoadAssetAsync Simulated: loading asset: " + name).Colored(Colors.Yellow));
                var comp = _simulatedAsset.GetComponent<T>();
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                return comp;
            }
#endif

            Debug.Log(("LoadAssetAsync: loading asset: " + name).Colored(Colors.Yellow));
            return await GetAssetComponentAsync<T>(Bundle.LoadAssetAsync(name), progress, cancellationToken);
        }

        /// <summary>
        /// Operation extracts an asset from the loaded bundle
        /// </summary>
        /// <param name="asyncOperation"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async UniTask<T> GetAssetComponentAsync<T>(AssetBundleRequest asyncOperation, IProgress<float> progress,
            CancellationToken cancellationToken) where T : UnityEngine.Object
        {
            while (!asyncOperation.isDone)
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                //Supressing this so it doesnt step over GetBundleFromStreamingAssetsAsync or GetBundleFromWebOrCacheAsync
                //progress?.Report(asyncOperation.progress);
                Debug.Log($"GetAssetComponentAsync {Bundle.name} progress: {asyncOperation.progress * 100f}%".Colored(Colors.LightSalmon));
            }

            if (!asyncOperation.asset)
                throw new Exception("RunAssetBundleRequestOperation: Error getting bundle.");

            //Current use case returns the component, if this changes then deal with it downstream but for now this should be ok
            var go = asyncOperation.asset as GameObject;
            return go.GetComponent<T>();
        }
    }
}