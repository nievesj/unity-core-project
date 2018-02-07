using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Levels
{
	public class LevelLoaderServiceConfiguration : ServiceConfiguration
	{
		override protected IService ServiceClass { get { return new LevelLoaderService(); } }

		public List<string> levels;
	}
}