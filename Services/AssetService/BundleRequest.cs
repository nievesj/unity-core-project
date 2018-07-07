using System;
using UnityEngine;

namespace Core.Services.Assets
{
    public struct AssetOptions
    {
        private AssetCacheState _assetCacheState;
        public AssetCacheState AssetCacheState => _assetCacheState;

        public AssetOptions(AssetCacheState sta)
        {
            _assetCacheState = sta;
        }
    }

    /// <summary>
    /// Helper class used to create a bundle request. Contains all the paths needed to request and
    /// access the bundle.
    /// </summary>
    public class BundleRequest
    {
        //Directory where the bundle is located.
        public AssetCategoryRoot AssetCategory { get; }

        public string BundleName { get; }
        
        public string AssetName { get; }

        //Manifest file associated to the bundle. This is needed in case the HASH number is requiered for caching the bundle
        [Obsolete("Deprecated. No longer needed.")]
        public string ManifestName => BundleName + ".manifest";

        [Obsolete("Deprecated. No longer needed.")]
        public string ManifestAgeFile => Application.persistentDataPath + "/" + BundleName + "age.json";
        
        [Obsolete("Deprecated. No longer needed.")]
        public string CachedManifestFile => Application.persistentDataPath + "/" + ManifestName;

        public string AssetPathFromLocalStreamingAssets
        {
            get
            {
                if (AssetCategory.Equals(AssetCategoryRoot.None))
                    return BundleName;
                else
                    return AssetCategory.ToString().ToLower() + "/" + BundleName;
            }
        }

        public string AssetPathFromLocalStreamingAssetsManifest
        {
            get
            {
                if (AssetCategory.Equals(AssetCategoryRoot.None))
                    return ManifestName;
                else
                    return AssetCategory.ToString().ToLower() + "/" + ManifestName;
            }
        }

        public BundleRequest(AssetCategoryRoot cat, string bundle, string asset)
        {
            AssetCategory = cat;
            BundleName = bundle.ToLower();
            AssetName = asset.ToLower();
        }
        
        public string GetAssetPath(AssetServiceConfiguration config)
        {
            if (AssetCategory.Equals(AssetCategoryRoot.None))
                return config.PlatformAssetBundleURL + BundleName + "?r=" + UnityEngine.Random.value * 9999999; //this random value prevents caching on the web server
            else
                return config.PlatformAssetBundleURL + AssetCategory.ToString().ToLower() + "/" + BundleName;
        }

        public string GetManifestPath(AssetServiceConfiguration config)
        {
            Debug.Log(("AssetBundleLoader: Loading Manifest " + ManifestName).Colored(Colors.Aqua));

            if (AssetCategory.Equals(AssetCategoryRoot.None))
                return config.PlatformAssetBundleURL + ManifestName + "?r=" + UnityEngine.Random.value * 9999999; //this random value prevents caching on the web server;
            else
                return config.PlatformAssetBundleURL + AssetCategory.ToString().ToLower() + "/" + ManifestName;
        }
    }

    /// <summary>
    /// Bundle root or package containing the desired asset
    /// </summary>
    public enum AssetCategoryRoot
    {
        None,
        Configuration,
        Services,
        Levels,
        Scenes,
        UI,
        Audio,
        Prefabs,
        Particles
    }

    /// <summary>
    /// Device type
    /// </summary>
    public enum AssetDeviceType
    {
        StandaloneOSXUniversal,
        StandaloneOSXIntel,
        StandaloneWindows,
        WebPlayer,
        WebPlayerStreamed,
        iOS,
        PS3,
        XBOX360,
        Android,
        StandaloneLinux,
        StandaloneWindows64,
        WebGL,
        WSAPlayer,
        StandaloneLinux64,
        StandaloneLinuxUniversal,
        WP8Player,
        StandaloneOSX,
        BlackBerry,
        Tizen,
        PSP2,
        PS4,
        PSM,
        XboxOne,
        SamsungTV,
        N3DS,
        WiiU,
        tvOS,
        Switch
    }

    /// <summary>
    /// Cache or no cache
    /// </summary>
    public enum AssetCacheState
    {
        Cache,
        NoCache
    }

    /// <summary>
    /// Used to determine if the caching is going to be performed with the Unity Cloud Manifest file
    /// by using the build version as the control or by using .manifest files and the HASH number
    /// </summary>
    public enum AssetCacheStrategy
    {
        CopyBundleManifestFileLocally,
        UseUnityCloudManifestBuildVersion
    }
}