namespace Core.Services.UI
{
	public class UIServiceConfiguration : ServiceConfiguration
	{
		public UIContainer mainCanvas;

		public override Service ServiceClass { get { return new UIService(this); } }
	}
}