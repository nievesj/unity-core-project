using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services.Assets
{
    /// <summary>
    /// Central point to get asset bundles required to run the game.
    /// </summary>
    public class AssetService : Service
    {
        [Inject]
        private AssetBundleLoader _assetBundlebundleLoader;

        public bool UseStreamingAssets => Configuration.UseStreamingAssets;

        public AssetCacheState AssetCacheState => Configuration.UseCache ? AssetCacheState.Cache : AssetCacheState.NoCache;

        public AssetCacheStrategy AssetCacheStrategy => AssetCacheStrategy.UseUnityCloudManifestBuildVersion;

        private UnityCloudBuildManifest _cloudBuildManifest;

        public UnityCloudBuildManifest CloudBuildManifest => _cloudBuildManifest;

        public readonly AssetServiceConfiguration Configuration;

        public AssetService(ServiceConfiguration config)
        {
            Configuration = config as AssetServiceConfiguration;

            UnityCloufBuildManifestLoader.LoadBuildManifest()
                .Subscribe(cloudManifest =>
                {
                    if (cloudManifest != null)
                    {
                        Debug.Log(("---- AssetService: Unity Cloud Build Manifest present. Build Version: " + cloudManifest.buildNumber).Colored(Colors.Aqua));
                        _cloudBuildManifest = cloudManifest;
                    }
                    else
                    {
                        Debug.Log("---- AssetService: Unity Cloud Build Manifest missing. This is ok. Ignoring.".Colored(Colors.Aqua));
                    }
                });
        }

        /// <summary>
        /// Gets and loads the required asset bundle
        /// </summary>
        /// <param name="bundleRequest"> Bundle to request </param>
        /// <returns> Observable </returns>
        public async Task<T> GetAndLoadAsset<T>(BundleRequest bundleRequest) where T : UnityEngine.Object
        {
            return await _assetBundlebundleLoader.LoadAsset<T>(bundleRequest);
        }

        /// <summary>
        /// Unloads asset and removes it from memory
        /// </summary>
        /// <param name="name">Asset name </param>
        /// <param name="unloadAllDependencies"> Unload all dependencies? </param>
        public void UnloadAsset(string name, bool unloadAllDependencies)
        {
            _assetBundlebundleLoader.UnloadAsset(name, unloadAllDependencies);
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