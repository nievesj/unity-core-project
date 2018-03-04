using System.Collections.Generic;

namespace Core.Services.Levels
{
	public class LevelLoaderServiceConfiguration : ServiceConfiguration
	{
		public override Service ServiceClass { get { return new LevelLoaderService(this); } }

		public List<string> levels;
	}
}