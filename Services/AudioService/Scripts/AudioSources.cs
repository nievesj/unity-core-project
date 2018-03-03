using Core.Polling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.Services.Audio
{
	public class AudioSources
	{
		[Inject]
		private Pooler<AudioSource> poller;

		private AudioSource prefab;

		public void Initialize(AudioSource obj, int pollingAmount)
		{
			//poller.Initialize(obj.gameObject, pollingAmount);
		}
	}
}