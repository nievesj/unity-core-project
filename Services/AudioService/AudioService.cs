using System.Collections;
using System.Collections.Generic;
using Core.Polling;
using Core.Service;
using Core.Signals;
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
		protected Services app;
		protected Poller<AudioSource> poller;

		protected Signal<IService> serviceConfigured = new Signal<IService>();
		public Signal<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Signal<IService> serviceStarted = new Signal<IService>();
		public Signal<IService> ServiceStarted { get { return serviceStarted; } }

		protected Signal<IService> serviceStopped = new Signal<IService>();
		public Signal<IService> ServiceStopped { get { return serviceStopped; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as AudioServiceConfiguration;
			serviceConfigured.Dispatch(this);
		}

		public void StartService(Services application)
		{
			app = application;
			Services.OnGameStart.Add(OnGameStart);
			serviceStarted.Dispatch(this);
		}

		public void StopService(Services application)
		{
			if (poller != null)
				poller.Destroy();

			serviceStopped.Dispatch(this);
		}

		protected void OnGameStart(Services application)
		{
			if (configuration.audioSourcePrefab)
			{
				poller = new Poller<AudioSource>(configuration.audioSourcePrefab.gameObject, configuration.poolAmount);
			}
			else
				Debug.LogError("AudioService : OnGameStart - Failed to create pool. Configuration is missing the AudioSource prefab.");
		}

		public void PlayClip(AudioPlayer ap)
		{
			Play(ap);
			Services.Instance.StartCoroutine(WaitUntilDonePlaying(ap));
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
				ap.Player.transform.SetParent(poller.PollerTransform);
				ap.Player.transform.localPosition = Vector3.zero;
			}

			poller.Push(ap.Player);
			ap.Player.clip = null;
			ap.Player = null;
		}

		protected IEnumerator WaitUntilDonePlaying(AudioPlayer ap)
		{
			CustomYieldInstruction wait = new WaitUntil(() => ap.Player.clip.loadState == AudioDataLoadState.Loaded);
			yield return wait;

			wait = new WaitWhile(() => ap.Player.isPlaying);
			yield return wait;

			Debug.Log(("AudioService: Done Playing Clip - " + ap.Clip.name).Colored(Colors.magenta));
			PushAudioSource(ap);
		}
	}
}