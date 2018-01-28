using System.Collections;
using System.Collections.Generic;
using Core.Polling;
using UnityEngine;

namespace Core.Audio
{
	public class AudioSources
	{
		Pooler<AudioSource> poller;
		AudioSource prefab;

		public void Initialize(AudioSource obj, int pollingAmount)
		{
			poller = new Pooler<AudioSource>(obj.gameObject, pollingAmount);
		}

	}
}