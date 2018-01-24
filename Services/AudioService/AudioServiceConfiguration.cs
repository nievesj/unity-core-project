using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UnityEngine;

namespace Core.Audio
{
	public class AudioServiceConfiguration : ServiceConfiguration
	{
		override protected IService ServiceClass { get { return new AudioService(); } }

		public AudioSource audioSourcePrefab;
		public int poolAmount = 10;
	}
}