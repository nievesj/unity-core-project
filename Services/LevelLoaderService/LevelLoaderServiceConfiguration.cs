using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UnityEngine;

namespace Core.LevelLoaderService
{
	public class LevelLoaderServiceConfiguration : ServiceConfiguration
	{
		override protected IService ServiceClass { get { return new LevelLoaderService(); } }

		public List<string> levels;
	}
}