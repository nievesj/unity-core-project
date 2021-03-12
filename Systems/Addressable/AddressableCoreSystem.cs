using System;
using System.Threading;
using Core.Common.Extensions.String;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Logger = UnityLogger.Logger;

namespace Core.Systems
{
    public enum AssetReferenceType
    {
        None,
        Scene,
        Prefab,
        GameObject,
        Component
    }

    [System.Serializable]
    public struct AddressableAssetReference
    {
        public AssetReferenceType assetReferenceType;
        public AssetReference reference;
    }

    public class AddressableCoreSystem : CoreSystem
    {
        /// <summary>
        ///  Checks if a scene is loaded, if it is the current scene is returned in scene.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="scene"></param>
        /// <returns></returns>
        public bool IsSceneLoaded(string sceneName, out Scene scene)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                scene = SceneManager.GetSceneAt(i);
                if (sceneName == scene.name)
                    return true;
            }

            scene = default;
            return false;
        }

        public void SetActiveScene(Scene scene)
        {
            SceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// Loads an asset from an AssetReference. Textures, Materials, Sound Clips, etc.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async UniTask<T> LoadAssetFromReferenceAsync<T>(AssetReference reference, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : UnityEngine.Object
        {
            var asyncOperation = reference.LoadAssetAsync<T>();
            var asset = await GetAsyncOperationResultAsync(asyncOperation, progress, cancellationToken);

            Logger.Log($"Loaded asset: {asset.name}".ColoredLog(Colors.Pink));
            return asset;
        }

        /// <summary>
        /// Loads a component from a prefab AssetReference.
        /// Use this when a component is needed from a prefab. (Particle System, Canvas, Image, etc)
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async UniTask<T> LoadPrefabAssetFromReferenceAsync<T>(AssetReference reference, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : UnityEngine.Object
        {
            var asset = await LoadAssetFromReferenceAsync<GameObject>(reference, progress, cancellationToken);

            Logger.Log($"Loaded component: {asset.name}".ColoredLog(Colors.Pink));
            return asset.GetComponent<T>();
        }

        /// <summary>
        /// Loads a scene from an AssetReference
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="loadSceneMode"></param>
        /// <param name="activateOnLoad"></param>
        /// <param name="priority"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async UniTask<Scene> LoadSceneFromReferenceAsync(AssetReference reference, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            var asyncOperation = Addressables.LoadSceneAsync(reference, loadSceneMode, activateOnLoad, priority);
            var sceneInstance = await GetAsyncOperationResultAsync(asyncOperation, progress, cancellationToken);

            Logger.Log($"Loaded Scene: {sceneInstance.Scene.name}".ColoredLog(Colors.Pink));

            return sceneInstance.Scene;
        }

        public async UniTask<T> LoadAssetByKey<T>(object key, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : UnityEngine.Object
        {
            var asyncOperation = Addressables.LoadAssetAsync<T>(key);
            var asset = await GetAsyncOperationResultAsync(asyncOperation, progress, cancellationToken);
            return asset;
        }

        private async UniTask<T> GetAsyncOperationResultAsync<T>(AsyncOperationHandle<T> asyncOperation, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            while (!asyncOperation.IsDone && !cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                progress?.Report(asyncOperation.PercentComplete);
            }

            if (asyncOperation.Status == AsyncOperationStatus.Failed)
                throw new Exception($"AssetBundleLoader:  {asyncOperation.OperationException}");

            if (cancellationToken.IsCancellationRequested)
                throw new Exception($"AssetBundleLoader:  Operation cancelled.");

            return asyncOperation.Result;
        }
    }
}