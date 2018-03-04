namespace Core.Services.UpdateManager
{
	public class UpdateServiceConfiguration : ServiceConfiguration
	{
		public Core.Services.UpdateManager.UpdateManager updateManager;

		public override Service ServiceClass { get { return new UpdateService(this); } }
	}
}