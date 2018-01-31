using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UnityEngine;

namespace Core.Scenes
{
	public class SceneLoaderServiceConfiguration : ServiceConfiguration
	{
		override protected IService ServiceClass { get { return new SceneLoaderService(); } }
	}
}