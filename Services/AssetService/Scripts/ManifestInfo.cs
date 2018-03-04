using System;
using System.Collections;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Services.Assets
{
	/// <summary>
	/// Class contains the relevant information contained on a manifest file and it's used for
	/// caching bundles /// TODO: store list of dependencies
	/// </summary>
	public class ManifestInfo
	{
		private uint crc;
		private int version;
		private string hash;
		private BundleRequest bundle;
		private int _manifestCacheExpirePeriod;
		public uint CRC { get { return crc; } }
		public int Version { get { return version; } }
		public Hash128 Hash { get { return Hash128.Parse(hash); } }

		public ManifestInfo(BundleRequest bundleNeeded, int period)
		{
			bundle = bundleNeeded;
			_manifestCacheExpirePeriod = period;
		}

		/// <summary>
		/// Gets .manifest file information
		/// </summary>
		/// <returns></returns>
		public IObservable<ManifestInfo> GetInfo()
		{
			return Observable.Create<ManifestInfo>(
				(IObserver<ManifestInfo> observer) =>
				{
					var subject = new Subject<ManifestInfo>();

					Action<string> OnManifestCacheWebLoaded = man =>
					{
						try { Create(man); }
						catch (Exception e) { observer.OnError(e); }

						observer.OnNext(this);
						observer.OnCompleted();
					};

					Action<bool> OnManifestExpiredCheck = isExpired =>
					{
						if (isExpired)
						{
							Action<string> OnManifestWebLoaded = loadedManifest =>
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

					IsManifestExpired(_manifestCacheExpirePeriod).Subscribe(OnManifestExpiredCheck);

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

				string[] readtext = text.Split("\n"[0]);

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
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		private IObservable<string> CacheManifest(string man)
		{
			return Observable.Create<string>(
				(IObserver<string> observer) =>
				{
					Debug.Log(("ManifestInfo: Caching Manifest | " + bundle.ManifestName).Colored(Colors.Brown));

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
				(IObserver<string> observer) =>
				{
					Debug.Log(("ManifestInfo: Getting Cached Manifest | " + bundle.ManifestName).Colored(Colors.Brown));

					var subject = new Subject<string>();
					var fileContents = string.Empty;

					try { fileContents = OpenFile(bundle.CachedManifestFile); }
					catch (Exception ex) { observer.OnError(ex); }

					observer.OnNext(fileContents);
					observer.OnCompleted();

					return subject.Subscribe();
				});
		}

		/// <summary>
		/// Observable wrapper for GetManifestOperation
		/// </summary>
		/// <returns> Observable </returns>
		private IObservable<string> GetManifestFromWeb()
		{
			return Observable.FromCoroutine<string>((observer, cancellationToken) => GetManifestOperation(observer, cancellationToken));
		}

		/// <summary>
		/// Gets manifest file from web.
		/// </summary>
		/// <param name="observer">          Observer </param>
		/// <param name="cancellationToken"> Cancellation token </param>
		/// <returns> IEnumerator </returns>
		private IEnumerator GetManifestOperation(IObserver<string> observer, CancellationToken cancellationToken)
		{
			Debug.Log(("ManifestInfo: Downloading Manifest | " + bundle.ManifestName).Colored(Colors.Brown));

			UnityWebRequest www = UnityWebRequest.Get(bundle.ManifestPath);
			yield return www.SendWebRequest();

			if (www.isNetworkError)
				observer.OnError(new Exception("AssetBundleLoader: " + www.error));

			observer.OnNext(www.downloadHandler.text);
			observer.OnCompleted();

			www.Dispose();
		}

		/// <summary>
		/// Determines of a cached manifest file is expired and needs to be refreshed
		/// </summary>
		/// <param name="expirationDays"> Days in which a file is considered expired </param>
		/// <returns> Observable </returns>
		private IObservable<bool> IsManifestExpired(int expirationDays)
		{
			return Observable.Create<bool>(
				(IObserver<bool> observer) =>
				{
					var subject = new Subject<string>();
					bool ret = false;

					if (!File.Exists(bundle.ManifestAgeFile))
					{
						Debug.Log(("ManifestInfo: No manifest age file. Flagging as expired | " + bundle.ManifestName).Colored(Colors.Brown));

						try { SaveAgeManifest(); }
						catch (Exception ex) { observer.OnError(ex); }

						ret = true;
					}
					else
					{
						var manifestDays = GetAgeManifest().Days;
						if (manifestDays > expirationDays)
						{
							Debug.Log(("ManifestInfo: Manifest file is longer than " + expirationDays + " days. Flagging as expired | " + bundle.ManifestName).Colored(Colors.Brown));

							try { SaveAgeManifest(); }
							catch (Exception ex) { observer.OnError(ex); }

							ret = true;
						}
						else
						{
							Debug.Log(("ManifestInfo: Manifest file still valid. Expires in " + (expirationDays - manifestDays) + " days | " + bundle.ManifestName).Colored(Colors.Brown));
						}
					}

					observer.OnNext(ret);
					observer.OnCompleted();

					return subject.Subscribe();
				});
		}

		/// <summary>
		/// Gets age of the manifest file, from the date it was created to the current system date
		/// </summary>
		/// <returns> TimeSpan in days </returns>
		private TimeSpan GetAgeManifest()
		{
			var fileContents = OpenFile(bundle.ManifestAgeFile);

			DateTime timeFromJson = JsonUtility.FromJson<JsonDateTime>(fileContents);
			TimeSpan diff = DateTime.Now.Subtract(timeFromJson);

			return diff;
		}

		/// <summary>
		/// Saves manifest age file. This is just a simple json file with a date
		/// </summary>
		private void SaveAgeManifest()
		{
			string jsonData = JsonUtility.ToJson((JsonDateTime)DateTime.Now, true);
			SaveFile(bundle.ManifestAgeFile, jsonData);
		}

		/// <summary>
		/// Saves a text file
		/// </summary>
		/// <param name="file"> File path, name should be included </param>
		/// <param name="data"> Data </param>
		private void SaveFile(string file, string data)
		{
			try
			{
				using (StreamWriter writer =
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

		/// <summary>
		/// Reads a text file
		/// </summary>
		/// <param name="file"> File path, name should be included </param>
		/// <returns> File contents </returns>
		private string OpenFile(string file)
		{
			try
			{
				using (StreamReader reader =
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