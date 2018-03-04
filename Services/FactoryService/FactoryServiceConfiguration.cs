namespace Core.Services.Factory
{
	public class FactoryServiceConfiguration : ServiceConfiguration
	{
		public override Service ServiceClass { get { return new FactoryService(this, null); } }
	}
}
