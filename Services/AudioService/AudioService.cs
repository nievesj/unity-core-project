using System.Collections;
using System.Collections.Generic;
using Core.Polling;
using Core.Service;
using UniRx;
using UnityEngine;

namespace Core.Audio
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

		protected Subject<IService> serviceConfigured = new Subject<IService>();
		public IObservable<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Subject<IService> serviceStarted = new Subject<IService>();
		public IObservable<IService> ServiceStarted { get { return serviceStarted; } }

		protected Subject<IService> serviceStopped = new Subject<IService>();
		public IObservable<IService> ServiceStopped { get { return serviceStopped; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as AudioServiceConfiguration;
			serviceConfigured.OnNext(this);
		}

		public void StartService()
		{
			ServiceLocator.OnGameStart.Subscribe(OnGameStart);
			serviceStarted.OnNext(this);
		}

		public void StopService()
		{
			if (poller != null)
				poller.Destroy();

			serviceStopped.OnNext(this);

			serviceConfigured.Dispose();
			serviceStarted.Dispose();
			serviceStopped.Dispose();
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