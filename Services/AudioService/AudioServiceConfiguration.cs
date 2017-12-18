using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UnityEngine;

namespace Core.Audio
{
	public class AudioServiceConfiguration : ServiceConfiguration
	{
		public AudioSource audioSourcePrefab;
		public int poolAmount = 10;

		protected override IService GetServiceClass()
		{
			return new AudioService();
		}

		public override void ShowEditorUI() {}
	}
}