using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UnityEngine;

namespace Core.LevelLoaderService
{
	public class LevelLoaderServiceConfiguration : ServiceConfiguration
	{
		public List<string> levels;

		protected override IService GetServiceClass()
		{
			return new LevelLoaderService();
		}
	}
}