using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Assets
{
	public class AssetServiceConfiguration : ServiceConfiguration
	{
		public override Service ServiceClass { get { return new AssetService(this); } }

		[SerializeField]
		protected string assetBundlesURL;

		public string AssetBundlesURL { get { return assetBundlesURL; } set { assetBundlesURL = value; } }

		[SerializeField]
		protected bool useStreamingAssets = false;

		public bool UseStreamingAssets { get { return useStreamingAssets; } set { useStreamingAssets = value; } }

		[SerializeField]
		protected bool useCache = true;

		public bool UseCache { get { return useCache; } set { useCache = value; } }

		[SerializeField]
		protected bool cacheBundleManifestsLocally = true;

		public bool CacheBundleManifestsLocally { get { return cacheBundleManifestsLocally; } set { cacheBundleManifestsLocally = value; } }

		[SerializeField]
		protected int manifestCachePeriod = 5;

		public int ManifestCachePeriod { get { return manifestCachePeriod; } set { manifestCachePeriod = value; } }

		[SerializeField]
		protected bool useUnityCloudBuildManifestVersion = true;

		public bool UseUnityCloudBuildManifestVersion { get { return useUnityCloudBuildManifestVersion; } set { useUnityCloudBuildManifestVersion = value; } }
	}
}