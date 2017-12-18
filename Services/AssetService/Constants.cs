using UnityEngine;

namespace Core.Assets
{
	public class Constants
	{

	}

	public struct AssetOptions
	{

		private AssetLoadProcess assetLoadProcess;
		public AssetLoadProcess AssetLoadProcess { get { return assetLoadProcess; } }

		private AssetCacheState assetCacheState;
		public AssetCacheState AssetCacheState { get { return assetCacheState; } }

		public AssetOptions(AssetLoadProcess pro, AssetCacheState sta)
		{
			assetLoadProcess = pro;
			assetCacheState = sta;
		}
	}

	public struct BundleNeeded
	{
		private AssetCategoryRoot assetCategory;
		public AssetCategoryRoot AssetCategory { get { return assetCategory; } }

		private AssetOptions options;
		public AssetOptions Options { get { return options; } }

		private string bundleName;
		public string BundleName { get { return bundleName; } }

		public string ManifestName { get { return bundleName + ".manifest"; } }

		private string assetName;
		public string AssetName { get { return assetName; } }

		public BundleNeeded(AssetCategoryRoot cat, string bundle, string asset)
		{
			assetCategory = cat;
			bundleName = bundle;
			assetName = asset;
			options = new AssetOptions(AssetLoadProcess.LoadSingleAsync, AssetCacheState.Cache);
		}

		public BundleNeeded(AssetCategoryRoot cat, AssetOptions opts, string bundle, string asset)
		{
			assetCategory = cat;
			bundleName = bundle;
			assetName = asset;
			options = opts;
		}
	}

	public struct ManifestInfo
	{
		private uint crc;
		private int version;
		private string hash;

		public uint CRC { get { return crc; } }
		public int Version { get { return version; } }
		public Hash128 Hash { get { return Hash128.Parse(hash); } }

		public ManifestInfo(string text)
		{
			crc = 0;
			version = 0;
			hash = string.Empty;

			string[] readtext = text.Split("\n" [0]);

			for (int i = 0; i < readtext.Length; i++)
				readtext[i].Trim();

			string[] crcline = readtext[1].Split(':');
			if (crcline.Length > 0)
				uint.TryParse(crcline[1].Trim(), out crc);

			string[] versionline = readtext[4].Split(':');
			if (versionline.Length > 0)
				int.TryParse(versionline[1].Trim(), out version);

			string[] hashline = readtext[5].Split(':');
			if (hashline.Length > 0)
				hash = hashline[1].Trim();
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
		SceneContent,
		GameContent,
		Windows,
		Audio
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
		StandaloneOSXIntel64,
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
	/// Loading process
	/// </summary>
	public enum AssetLoadProcess
	{
		LoadSingleSync,
		LoadSingleAsync,
		LoadAllSync,
		LoadAllAsync,
	}

	/// <summary>
	/// Cache or no cache
	/// </summary>
	public enum AssetCacheState
	{
		Cache,
		NoCache
	}
}