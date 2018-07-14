namespace Core.Services.Scenes
{
    public class SceneLoaderServiceConfiguration : ServiceConfiguration
    {
        public override Service ServiceClass => new SceneLoaderService(this);
    }
}