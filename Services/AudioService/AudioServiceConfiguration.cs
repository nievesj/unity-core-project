using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Audio
{
	public class AudioServiceConfiguration : ServiceConfiguration
	{
		public override Service ServiceClass { get { return new AudioService(this); } }

		public AudioSource audioSourcePrefab;

		public int poolAmount = 10;
	}
}