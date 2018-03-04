using Core.Services.Factory;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services.Audio
{
	public class AudioService : Service
	{
		[Inject]
		private FactoryService _factoryService;

		private Pooler<AudioSource> _pooler;

		private AudioServiceConfiguration _configuration;

		private List<AudioPlayer> _activeAudioPlayers;

		//Global _mute
		private bool _mute;

		public bool Mute
		{
			get { return _mute; }
			set
			{
				_mute = value;
				foreach (var ap in _activeAudioPlayers) ap.Player.mute = _mute;
			}
		}

		//Global _volume
		private float _volume;

		public float Volume
		{
			get { return _volume; }
			set
			{
				_volume = value;
				foreach (var ap in _activeAudioPlayers) ap.Player.volume = _volume;
			}
		}

		public AudioService(ServiceConfiguration config)
		{
			_configuration = config as AudioServiceConfiguration;
			_activeAudioPlayers = new List<AudioPlayer>();
		}

		public override void Initialize()
		{
			base.Initialize();

			if (_configuration.audioSourcePrefab)
			{
				_pooler = _factoryService.CreatePool<AudioSource>(_configuration.audioSourcePrefab.gameObject, _configuration.poolAmount);
			}
			else
				Debug.LogError("AudioService : PlayClip - Failed to create pool. Configuration is missing the AudioSource prefab.");
		}

		public void PlayClip(AudioPlayer ap)
		{
			Play(ap);

			MainThreadDispatcher.StartCoroutine(WaitUntilDonePlaying(ap));
		}

		public void PlayClip(AudioClip clip)
		{
			var ap = new AudioPlayer(clip);
			PlayClip(ap);
		}

		public void PlayMusic(AudioPlayer ap)
		{
			_activeAudioPlayers.Add(ap);
			Play(ap);
		}

		private void Play(AudioPlayer ap)
		{
			if (_pooler != null && (!ap.Player || !ap.Player.gameObject.activeSelf))
			{
				_activeAudioPlayers.Add(ap);

				if (ap.PlayFrom)
				{
					ap.Player.transform.SetParent(ap.PlayFrom);
					ap.Player.transform.localPosition = Vector3.zero;
				}

				Debug.Log(("AudioService: Playing Clip - " + ap.Clip.name).Colored(Colors.Magenta));
				ap.Player = _pooler.Pop();

				ap.Player.volume = _volume;
				ap.Player.mute = _mute;

				ap.Player.Play();
			}
		}

		public void StopClip(AudioPlayer ap)
		{
			if (_pooler != null && ap.Player)
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
				ap.Player.transform.SetParent(_pooler.PoolerTransform);
				ap.Player.transform.localPosition = Vector3.zero;
			}

			_activeAudioPlayers.Remove(ap);
			_pooler.Push(ap.Player);
			ap.Player.clip = null;
			ap.Player = null;
		}

		private IEnumerator WaitUntilDonePlaying(AudioPlayer ap)
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