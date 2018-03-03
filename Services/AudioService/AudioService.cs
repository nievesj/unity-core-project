using System.Collections;
using System.Collections.Generic;
using Core.Polling;
using Core.Services;
using UniRx;
using UnityEngine;

namespace Core.Services.Audio
{
	public interface IAudioService : IService
	{
		void PlayClip(AudioClip clip);

		void PlayClip(AudioPlayer ap);

		void PlayMusic(AudioPlayer ap);

		void StopClip(AudioPlayer ap);

		bool Mute { get; set; }
		float Volume { get; set; }
	}

	public class AudioService : IAudioService
	{
		protected AudioServiceConfiguration configuration;
		protected Pooler<AudioSource> poller;
		protected List<AudioPlayer> activeAudioPlayers;

		//Global mute
		private bool mute;

		public bool Mute
		{
			get { return mute; }
			set
			{
				mute = value;
				foreach (var ap in activeAudioPlayers) ap.Player.mute = mute;
			}
		}

		//Global volume
		private float volume;

		public float Volume
		{
			get { return volume; }
			set
			{
				volume = value;
				foreach (var ap in activeAudioPlayers) ap.Player.volume = volume;
			}
		}

		public AudioService(ServiceConfiguration config)
		{
			configuration = config as AudioServiceConfiguration;
			ServiceLocator.OnGameStart.Subscribe(OnGameStart);
			activeAudioPlayers = new List<AudioPlayer>();

			if (configuration.audioSourcePrefab)
			{
				poller = new Pooler<AudioSource>(configuration.audioSourcePrefab.gameObject, configuration.poolAmount);
			}
			else
				Debug.LogError("AudioService : OnGameStart - Failed to create pool. Configuration is missing the AudioSource prefab.");
		}

		protected void OnGameStart(ServiceLocator locator)
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

		public void PlayClip(AudioClip clip)
		{
			var ap = new AudioPlayer(clip);
			PlayClip(ap);
		}

		public void PlayMusic(AudioPlayer ap)
		{
			activeAudioPlayers.Add(ap);
			Play(ap);
		}

		protected void Play(AudioPlayer ap)
		{
			if (poller != null && (!ap.Player || !ap.Player.gameObject.activeSelf))
			{
				activeAudioPlayers.Add(ap);

				if (ap.PlayFrom)
				{
					ap.Player.transform.SetParent(ap.PlayFrom);
					ap.Player.transform.localPosition = Vector3.zero;
				}

				Debug.Log(("AudioService: Playing Clip - " + ap.Clip.name).Colored(Colors.Magenta));
				ap.Player = poller.Pop();

				ap.Player.volume = volume;
				ap.Player.mute = mute;

				ap.Player.Play();
			}
		}

		public void StopClip(AudioPlayer ap)
		{
			if (poller != null && ap.Player)
			{
				Debug.Log(("AudioService: Stopping Clip - " + ap.Clip.name).Colored(Colors.Magenta));

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

			activeAudioPlayers.Remove(ap);
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

			if (ap.Clip)
				Debug.Log(("AudioService: Done Playing Clip - " + ap.Clip.name).Colored(Colors.Magenta));

			PushAudioSource(ap);
		}
	}
}