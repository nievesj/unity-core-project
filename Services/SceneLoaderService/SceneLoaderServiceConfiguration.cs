using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Scenes
{
	public class SceneLoaderServiceConfiguration : ServiceConfiguration
	{
		public override Service ServiceClass { get { return new SceneLoaderService(this); } }
	}
}