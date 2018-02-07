using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Audio
{
	public class AudioServiceConfiguration : ServiceConfiguration
	{
		override protected IService ServiceClass { get { return new AudioService(); } }

		public AudioSource audioSourcePrefab;
		public int poolAmount = 10;
	}
}