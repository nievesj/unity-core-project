using UnityEngine;

namespace Core.Services.Audio
{
	[System.Serializable]
	public class AudioPlayer
	{
		[SerializeField]
		private readonly AudioClip _clip;

		public AudioClip Clip => _clip;

		[SerializeField]
		private readonly Transform _playFrom;

		public Transform PlayFrom => _playFrom;

		public float Pitch { get; set; }

		[SerializeField]
		private AudioSource audioSource;

		public AudioSource Player
		{
			get => audioSource;
			set
			{
				audioSource = value;
				if (audioSource)
				{
					audioSource.clip = _clip;
					SetUpOptions(audioSource);
				}
			}
		}

		[SerializeField]
		private readonly AudioSourceOptions _audioSourceOptions;

		public AudioSourceOptions AudioOptions => _audioSourceOptions;

		public AudioPlayer(AudioClip ac)
		{
			_clip = ac;
			_audioSourceOptions = new AudioSourceOptions();
		}

		public AudioPlayer(AudioClip ac, Transform from)
		{
			_clip = ac;
			_playFrom = from;
		}

		public AudioPlayer(AudioClip ac, GameObject from)
		{
			_clip = ac;
			_playFrom = from.transform; ;
		}

		private void SetUpOptions(AudioSource aus)
		{
			aus.mute = _audioSourceOptions.mute;
			aus.bypassEffects = _audioSourceOptions.bypassEffects;
			aus.bypassListenerEffects = _audioSourceOptions.bypassListenerEffects;
			aus.bypassReverbZones = _audioSourceOptions.bypassReverbZones;
			aus.playOnAwake = _audioSourceOptions.playOnAwake;
			aus.loop = _audioSourceOptions.loop;

			aus.priority = _audioSourceOptions.priority;
			aus.volume = _audioSourceOptions.volume;
			aus.pitch = _audioSourceOptions.pitch;
			aus.panStereo = _audioSourceOptions.panStereo;
			aus.spatialBlend = _audioSourceOptions.spatialBlend;
			aus.reverbZoneMix = _audioSourceOptions.reverbZoneMix;

			aus.dopplerLevel = _audioSourceOptions.SoundSettings3D.dopplerLevel;
			aus.spread = _audioSourceOptions.SoundSettings3D.spread;
			aus.rolloffMode = _audioSourceOptions.SoundSettings3D.rolloffMode;
			aus.minDistance = _audioSourceOptions.SoundSettings3D.minDistance;
			aus.maxDistance = _audioSourceOptions.SoundSettings3D.maxDistance;
		}
	}
}