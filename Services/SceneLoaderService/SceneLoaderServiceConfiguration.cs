namespace Core.Services.Scenes
{
	public class SceneLoaderServiceConfiguration : ServiceConfiguration
	{
		public override Service ServiceClass { get { return new SceneLoaderService(this); } }
	}
}