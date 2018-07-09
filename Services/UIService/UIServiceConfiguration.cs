namespace Core.Services.UI
{
	public class UIServiceConfiguration : ServiceConfiguration
	{
		public UIContainer mainCanvas;

		public UIScreenBlocker UIScreenBlocker;

		public override Service ServiceClass { get { return new UIService(this); } }
	}
}