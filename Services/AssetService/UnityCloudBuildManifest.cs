using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Core.Services.Assets
{
	/// <summary>
	/// Attempts to load the file UnityCloudBuildManifest.json. This file is added to the build when
	/// using Unity Cloud Build services.
	///
	/// https://docs.unity3d.com/Manual/UnityCloudBuildManifest.html
	/// </summary>
	[System.Serializable]
	public class UnityCloudBuildManifest
	{
		public string scmCommitId;
		public string scmBranch;
		public string buildNumber;
		public string buildStartTime;
		public string projectId;
		public string bundleId;
		public string unityVersion;
		public string xcodeVersion;
		public string cloudBuildTargetName;
	}

	public static class UnityCloufBuildManifestLoader
	{
		/// <summary>
		/// Loads UnityCloudBuildManifest into a structure the rest of the system can use,
		/// </summary>
		/// <returns></returns>
		public static async Task<UnityCloudBuildManifest> LoadBuildManifest()
		{
			var res = await Resources.LoadAsync(Constants.UnityBuildManifest);
			UnityCloudBuildManifest manifest = null;
			if (res)
			{
				var text = res as TextAsset;
				if (text != null)
					manifest = JsonUtility.FromJson<UnityCloudBuildManifest>(text.text);
			}

			return manifest;
		}
	}
}