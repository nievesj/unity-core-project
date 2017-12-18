using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio
{
	[System.Serializable]
	public class AudioSourceOptions
	{
		public AudioMixerGroup output;
		public bool mute;
		public bool bypassEffects;
		public bool bypassListenerEffects;
		public bool bypassReverbZones;
		public bool playOnAwake = true;
		public bool loop;

		[Range(0, 256)]
		public int priority = 128;

		[Range(0, 1)]
		public float volume = 1;

		[Range(-3, 3)]
		public float pitch = 1;

		[Range(-1, 1)]
		public float panStereo = 0;

		[Range(0, 1)]
		public float spatialBlend = 0;

		[Range(0, 1.1f)]
		public float reverbZoneMix = 1;

		public SoundSettings SoundSettings3D;
	}

	[System.Serializable]
	public class SoundSettings
	{
		[Range(0, 5)]
		public float dopplerLevel = 1;

		[Range(0, 360)]
		public float spread = 0;
		public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
		public float minDistance = 1f;
		public float maxDistance = 500;
	}
}