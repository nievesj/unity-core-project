using Core.Polling;
using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.Audio
{

	public interface IAudioService : IService
	{
		void PlayClip(AudioPlayer ap);
		void PlayMusic(AudioPlayer ap);
		void StopClip(AudioPlayer ap);
	}

	public class AudioService : IAudioService
	{
		protected AudioServiceConfiguration configuration;
		protected Pooler<AudioSource> poller;

		public IObservable<IService> Configure(ServiceConfiguration config)
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					configuration = config as AudioServiceConfiguration;
					ServiceLocator.OnGameStart.Subscribe(OnGameStart);

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		public IObservable<IService> StartService()
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		public IObservable<IService> StopService()
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					if (poller != null)
						poller.Destroy();

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		protected void OnGameStart(ServiceLocator application)
		{
			if (configuration.audioSourcePrefab)
			{
				poller = new Pooler<AudioSource>(configuration.audioSourcePrefab.gameObject, configuration.poolAmount);
			}
			else
				Debug.LogError("AudioService : OnGameStart - Failed to create pool. Configuration is missing the AudioSource prefab.");
		}

		public void PlayClip(AudioPlayer ap)
		{
			Play(ap);
			ServiceLocator.Instance.StartCoroutine(WaitUntilDonePlaying(ap));
		}

		public void PlayMusic(AudioPlayer ap)
		{
			Play(ap);
		}

		protected void Play(AudioPlayer ap)
		{
			if (poller != null && (!ap.Player || !ap.Player.gameObject.activeSelf))
			{
				if (ap.PlayFrom)
				{
					ap.Player.transform.SetParent(ap.PlayFrom);
					ap.Player.transform.localPosition = Vector3.zero;
				}

				Debug.Log(("AudioService: Playing Clip - " + ap.Clip.name).Colored(Colors.magenta));
				ap.Player = poller.Pop();
				ap.Player.Play();
			}
		}

		public void StopClip(AudioPlayer ap)
		{
			if (poller != null && ap.Player)
			{
				Debug.Log(("AudioService: Stopping Clip - " + ap.Clip.name).Colored(Colors.magenta));

				ap.Player.Stop();
				PushAudioSource(ap);
			}
		}

		protected void PushAudioSource(AudioPlayer ap)
		{
			if (ap.PlayFrom)
			{
				ap.Player.transform.SetParent(poller.PoolerTransform);
				ap.Player.transform.localPosition = Vector3.zero;
			}

			poller.Push(ap.Player);
			ap.Player.clip = null;
			ap.Player = null;
		}

		protected IEnumerator WaitUntilDonePlaying(AudioPlayer ap)
		{
			CustomYieldInstruction wait = new WaitUntil(()=> ap.Player.clip.loadState == AudioDataLoadState.Loaded);
			yield return wait;

			wait = new WaitWhile(()=> ap.Player.isPlaying);
			yield return wait;

			Debug.Log(("AudioService: Done Playing Clip - " + ap.Clip.name).Colored(Colors.magenta));
			PushAudioSource(ap);
		}
	}
}