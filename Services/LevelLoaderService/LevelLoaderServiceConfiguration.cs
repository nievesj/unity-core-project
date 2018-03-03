using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Levels
{
	public class LevelLoaderServiceConfiguration : ServiceConfiguration
	{
		protected override IService ServiceClass { get { return new LevelLoaderService(this); } }

		public List<string> levels;
	}
}