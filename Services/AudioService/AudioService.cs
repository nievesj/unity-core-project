using System.Collections;
using System.Collections.Generic;
using Core.Polling;
using Core.Services;
using Core.Services.Factory;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services.Audio
{
	public interface IAudioService : IService { }

	public class AudioService : IAudioService
	{
		[Inject]
		private FactoryService _factoryService;

		private Pooler<AudioSource> pooler;

		private AudioServiceConfiguration configuration;
		private List<AudioPlayer> activeAudioPlayers;

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
			activeAudioPlayers = new List<AudioPlayer>();

			if (configuration.audioSourcePrefab)
			{
				//pooler = _factoryService.CreatePool<AudioSource>(configuration.audioSourcePrefab.gameObject, configuration.poolAmount);
			}
			else
				Debug.LogError("AudioService : OnGameStart - Failed to create pool. Configuration is missing the AudioSource prefab.");
		}

		private void OnGameStart(ServiceLocator locator) { }

		public void PlayClip(AudioPlayer ap)
		{
			Play(ap);

			Observable.FromCoroutine<bool>((observer) => WaitUntilDonePlaying(ap, observer)).Subscribe();
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

		private void Play(AudioPlayer ap)
		{
			if (pooler != null && (!ap.Player || !ap.Player.gameObject.activeSelf))
			{
				activeAudioPlayers.Add(ap);

				if (ap.PlayFrom)
				{
					ap.Player.transform.SetParent(ap.PlayFrom);
					ap.Player.transform.localPosition = Vector3.zero;
				}

				Debug.Log(("AudioService: Playing Clip - " + ap.Clip.name).Colored(Colors.Magenta));
				ap.Player = pooler.Pop();

				ap.Player.volume = volume;
				ap.Player.mute = mute;

				ap.Player.Play();
			}
		}

		public void StopClip(AudioPlayer ap)
		{
			if (pooler != null && ap.Player)
			{
				Debug.Log(("AudioService: Stopping Clip - " + ap.Clip.name).Colored(Colors.Magenta));

				ap.Player.Stop();
				PushAudioSource(ap);
			}
		}

		private void PushAudioSource(AudioPlayer ap)
		{
			if (ap.PlayFrom)
			{
				ap.Player.transform.SetParent(pooler.PoolerTransform);
				ap.Player.transform.localPosition = Vector3.zero;
			}

			activeAudioPlayers.Remove(ap);
			pooler.Push(ap.Player);
			ap.Player.clip = null;
			ap.Player = null;
		}

		private IEnumerator WaitUntilDonePlaying(AudioPlayer ap, IObserver<bool> observer)
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