namespace Core.Services.Factory
{
    public class FactoryServiceConfiguration : ServiceConfiguration
    {
        public override Service ServiceClass => new FactoryService(this, null);
    }
}