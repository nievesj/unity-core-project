using System;
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

	public class UnityCloufBuildManifestLoader
	{
		/// <summary>
		/// Loads UnityCloudBuildManifest into a structure the rest of the system can use,
		/// </summary>
		/// <returns></returns>
		public static IObservable<UnityCloudBuildManifest> LoadBuildManifest()
		{
			return Observable.Create<UnityCloudBuildManifest>(
				(IObserver<UnityCloudBuildManifest> observer) =>
				{
					Action<ResourceRequest> OnResourceLoaded = resource =>
					{
						if (resource.asset)
						{
							var text = resource.asset as TextAsset;
							var manifest = JsonUtility.FromJson<UnityCloudBuildManifest>(text.text);

							observer.OnNext(manifest);
							observer.OnCompleted();
						}
						else
						{
							observer.OnNext(null);
							observer.OnCompleted();
						}
					};

					//FIXME: Make this a constant in case unity decides to change the name later, of the developer implements another process
					return Resources.LoadAsync("UnityCloudBuildManifest.json").AsAsyncOperationObservable<ResourceRequest>().Subscribe(OnResourceLoaded);
				});
		}
	}
}