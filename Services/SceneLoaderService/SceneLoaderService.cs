using System;
using System.Threading;
using Core.Common.Extensions.String;
using Core.Services.Assets;
using Core.Services.UI;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Logger = UnityLogger.Logger;

namespace Core.Services.Scenes
{
    public class SceneLoaderService : Service
    {
#pragma warning disable 0414    // suppress value not used warning
        private SceneLoaderServiceConfiguration _configuration;
#pragma warning restore 0414    // restore value not used warning

        [Inject]
        private AssetService _assetService;

        [Inject]
        private UIService _uiService;

        public SceneLoaderService(ServiceConfiguration config)
        {
            _configuration = config as SceneLoaderServiceConfiguration;
        }

        /// <summary>
        /// Attempts to load a scene from an asset bundle
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"> </param>
        /// <param name="forceLoadFromStreamingAssets"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async UniTask<UnityEngine.Object> LoadScene(string scene, LoadSceneMode mode = LoadSceneMode.Single,
            bool forceLoadFromStreamingAssets = false, IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            return await GetScene(scene, mode, forceLoadFromStreamingAssets, progress, cancellationToken);
        }

        /// <summary>
        /// Gets a scene from an asset bundle
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"> </param>
        /// <param name="forceLoadFromStreamingAssets"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async UniTask<UnityEngine.Object> GetScene(string scene, LoadSceneMode mode,
            bool forceLoadFromStreamingAssets, IProgress<float> progress,
            CancellationToken cancellationToken)
        {
            if (_assetService.GetLoadedBundle(scene))
                throw new Exception("Scene " + scene + " is already loaded and open. Opening the same scene twice is not supported.");

            var sceneObject = await _assetService.GetScene(new BundleRequest(AssetCategoryRoot.Scenes, scene, scene),
                forceLoadFromStreamingAssets, progress, cancellationToken);

            if (sceneObject && !cancellationToken.IsCancellationRequested)
            {
                Logger.Log(("SceneLoaderService: Loaded scene - " + scene),Colors.LightBlue);

                await SceneManager.LoadSceneAsync(scene, mode);
                Logger.Log(("SceneLoaderService: Opened scene - " + scene),Colors.LightBlue);
            }

            return sceneObject;
        }

        /// <summary>
        /// Unload scene.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public async UniTask UnLoadScene(string scene)
        {
            await SceneManager.UnloadSceneAsync(scene);
            await _assetService.UnloadAsset(scene, true);
            Logger.Log(("SceneLoaderService: Unloaded scene - " + scene),Colors.LightBlue);
        }

        public Scene GetLoadedScene(string sceneName)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                    return scene;
            }

            return default;
        }
    }
}