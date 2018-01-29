using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core.Assets;
using Core.Service;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Assets
{
	public class ManifestInfo
	{
		private uint crc;
		private int version;
		private string hash;
		private BundleRequest bundle;
		private int ManifestCacheExpirePeriod
		{
			get { return ServiceLocator.GetService<IAssetService>().ManifestCacheExpiringPeriodInDays; }
		}

		public uint CRC { get { return crc; } }
		public int Version { get { return version; } }
		public Hash128 Hash { get { return Hash128.Parse(hash); } }

		public ManifestInfo(BundleRequest bundleNeeded)
		{
			bundle = bundleNeeded;
		}

		public IObservable<ManifestInfo> GetInfo()
		{
			return Observable.Create<ManifestInfo>(
				(IObserver<ManifestInfo> observer)=>
				{
					var subject = new Subject<ManifestInfo>();

					System.Action<string> OnManifestCacheWebLoaded = man =>
					{
						Create(man);

						observer.OnNext(this);
						observer.OnCompleted();
					};

					System.Action<bool> OnManifestExpiredCheck = isExpired =>
					{
						if (isExpired)
						{
							System.Action<string> OnManifestWebLoaded = loadedManifest =>
							{
								CacheManifest(loadedManifest).Subscribe(OnManifestCacheWebLoaded);
							};
							GetManifestFromWeb().Subscribe(OnManifestWebLoaded);
						}
						else
						{
							GetCachedManifest().Subscribe(OnManifestCacheWebLoaded);
						}

						observer.OnNext(this);
						observer.OnCompleted();
					};
					IsManifestExpired(ManifestCacheExpirePeriod).Subscribe(OnManifestExpiredCheck);

					observer.OnNext(null);
					observer.OnCompleted();

					return subject.Subscribe();
				});
		}

		private void Create(string text)
		{
			try
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
			catch (System.Exception e)
			{
				Debug.LogError("ManifestInfo: Check URL | " + e.Message);
				Debug.LogError(text);
			}
		}

		private IObservable<string> CacheManifest(string man)
		{
			return Observable.Create<string>(
				(IObserver<string> observer)=>
				{
					Debug.Log(("ManifestInfo: Caching Manifest | " + bundle.ManifestName).Colored(Colors.brown));

					var subject = new Subject<string>();

					try { SaveFile(bundle.CachedManifestFile, man); }
					catch (Exception ex) { observer.OnError(ex); }

					observer.OnNext(man);
					observer.OnCompleted();

					return subject.Subscribe();
				});
		}

		private IObservable<string> GetCachedManifest()
		{
			return Observable.Create<string>(
				(IObserver<string> observer)=>
				{
					Debug.Log(("ManifestInfo: Getting Cached Manifest | " + bundle.ManifestName).Colored(Colors.brown));

					var subject = new Subject<string>();
					var fileContents = string.Empty;

					try { fileContents = OpenFile(bundle.CachedManifestFile); }
					catch (Exception ex) { observer.OnError(ex); }

					observer.OnNext(fileContents);
					observer.OnCompleted();

					return subject.Subscribe();
				});
		}

		private IObservable<string> GetManifestFromWeb()
		{
			return Observable.FromCoroutine<string>((observer, cancellationToken)=> GetManifestOperation(observer, cancellationToken));
		}

		private IEnumerator GetManifestOperation(IObserver<string> observer, CancellationToken cancellationToken)
		{
			Debug.Log(("ManifestInfo: Downloading Manifest | " + bundle.ManifestName).Colored(Colors.brown));

			UnityWebRequest www = UnityWebRequest.Get(bundle.ManifestPath);
			yield return www.SendWebRequest();

			if (www.isNetworkError)
				observer.OnError(new System.Exception("AssetBundleLoader: " + www.error));

			observer.OnNext(www.downloadHandler.text);
			observer.OnCompleted();

			www.Dispose();
		}

		private IObservable<bool> IsManifestExpired(int expiredDays)
		{
			return Observable.Create<bool>(
				(IObserver<bool> observer)=>
				{
					var subject = new Subject<string>();
					bool ret = false;

					if (!File.Exists(bundle.ManifestAgeFile))
					{
						Debug.Log(("ManifestInfo: No manifest age file. Flagging as expired | " + bundle.ManifestName).Colored(Colors.brown));

						try { SaveAgeManifest(); }
						catch (Exception ex) { observer.OnError(ex); }

						ret = true;
					}
					else
					{
						var manifestDays = GetAgeManifest().Days;
						if (manifestDays > expiredDays)
						{
							Debug.Log(("ManifestInfo: Manifest file is longer than " + expiredDays + " days. Flagging as expired | " + bundle.ManifestName).Colored(Colors.brown));

							try { SaveAgeManifest(); }
							catch (Exception ex) { observer.OnError(ex); }

							ret = true;
						}
						else
						{
							Debug.Log(("ManifestInfo: Manifest file still valid. Expires in " + (expiredDays - manifestDays)+ " days | " + bundle.ManifestName).Colored(Colors.brown));
						}
					}

					observer.OnNext(ret);
					observer.OnCompleted();

					return subject.Subscribe();
				});
		}

		private TimeSpan GetAgeManifest()
		{
			var fileContents = OpenFile(bundle.ManifestAgeFile);

			System.DateTime timeFromJson = JsonUtility.FromJson<JsonDateTime>(fileContents);
			System.TimeSpan diff = System.DateTime.Now.Subtract(timeFromJson);

			return diff;
		}

		private void SaveAgeManifest()
		{
			string jsonData = JsonUtility.ToJson((JsonDateTime)System.DateTime.Now, true);
			SaveFile(bundle.ManifestAgeFile, jsonData);
		}

		private void SaveFile(string file, string data)
		{
			try
			{
				using(StreamWriter writer =
					new StreamWriter(file))
				{
					writer.Write(data);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		private string OpenFile(string file)
		{
			try
			{
				using(StreamReader reader =
					new StreamReader(file))
				{
					return reader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}