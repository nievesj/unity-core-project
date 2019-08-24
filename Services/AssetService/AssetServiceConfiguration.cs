using UnityEngine;

namespace Core.Services.Assets
{
    public class AssetServiceConfiguration : ServiceConfiguration
    {
        public override Service ServiceClass => new AssetService(this);

        [SerializeField]
        private string _assetBundlesURL;

        public string AssetBundlesURL
        {
            get => _assetBundlesURL;
            set => _assetBundlesURL = value;
        }

        [SerializeField]
        private bool _useStreamingAssets = false;

        public bool UseStreamingAssets
        {
            get => _useStreamingAssets;
            set => _useStreamingAssets = value;
        }

        [SerializeField]
        private bool _useCache = true;

        public bool UseCache
        {
            get => _useCache;
            set => _useCache = value;
        }

        [SerializeField]
        private int _manifestCachePeriod = 5;

        public int ManifestCachePeriod
        {
            get => _manifestCachePeriod;
            set => _manifestCachePeriod = value;
        }

        [SerializeField]
        private bool _useUnityCloudBuildManifestVersion = true;

        public bool UseUnityCloudBuildManifestVersion
        {
            get => _useUnityCloudBuildManifestVersion;
            set => _useUnityCloudBuildManifestVersion = value;
        }

        public string PlatformAssetBundleURL => AssetBundlesURL + AssetBundleUtilities.ClientPlatform + "/";
    }
}