using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Audio
{
	[System.Serializable]
	public class AudioPlayer
	{
		[SerializeField]
		AudioClip clip;
		public AudioClip Clip { get { return clip; } }

		[SerializeField]
		Transform playFrom;
		public Transform PlayFrom { get { return playFrom; } }

		public float pitch { get; set; }

		[SerializeField]
		AudioSource audioSource;
		public AudioSource Player
		{
			get { return audioSource; }
			set
			{
				audioSource = value;

				if (audioSource)
				{
					audioSource.clip = clip;
					SetUpOptions(audioSource);
				}
			}
		}

		[SerializeField]
		AudioSourceOptions audioSourceOptions;
		public AudioSourceOptions AudioOptions { get { return audioSourceOptions; } }

		public AudioPlayer(AudioClip ac)
		{
			clip = ac;
			audioSourceOptions = new AudioSourceOptions();
		}

		public AudioPlayer(AudioClip ac, Transform from)
		{
			clip = ac;
			playFrom = from;
		}

		public AudioPlayer(AudioClip ac, GameObject from)
		{
			clip = ac;
			playFrom = from.transform;;
		}

		protected void SetUpOptions(AudioSource aus)
		{
			aus.mute = audioSourceOptions.mute;
			aus.bypassEffects = audioSourceOptions.bypassEffects;
			aus.bypassListenerEffects = audioSourceOptions.bypassListenerEffects;
			aus.bypassReverbZones = audioSourceOptions.bypassReverbZones;
			aus.playOnAwake = audioSourceOptions.playOnAwake;
			aus.loop = audioSourceOptions.loop;

			aus.priority = audioSourceOptions.priority;
			aus.volume = audioSourceOptions.volume;
			aus.pitch = audioSourceOptions.pitch;
			aus.panStereo = audioSourceOptions.panStereo;
			aus.spatialBlend = audioSourceOptions.spatialBlend;
			aus.reverbZoneMix = audioSourceOptions.reverbZoneMix;

			aus.dopplerLevel = audioSourceOptions.SoundSettings3D.dopplerLevel;
			aus.spread = audioSourceOptions.SoundSettings3D.spread;
			aus.rolloffMode = audioSourceOptions.SoundSettings3D.rolloffMode;
			aus.minDistance = audioSourceOptions.SoundSettings3D.minDistance;
			aus.maxDistance = audioSourceOptions.SoundSettings3D.maxDistance;
		}
	}
}