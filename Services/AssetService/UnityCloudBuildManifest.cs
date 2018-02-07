using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.Assets
{
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
		public static IObservable<UnityCloudBuildManifest> LoadBuildManifest()
		{
			return Observable.Create<UnityCloudBuildManifest>(
				(IObserver<UnityCloudBuildManifest> observer)=>
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