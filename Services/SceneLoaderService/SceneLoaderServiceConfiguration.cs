using System;

namespace Core.Services.Scenes
{
	[Obsolete("Deprecating because this is a glorified prefab loader.")]
	public class SceneLoaderServiceConfiguration : ServiceConfiguration
	{
		public override Service ServiceClass { get { return new SceneLoaderService(this); } }
	}
}