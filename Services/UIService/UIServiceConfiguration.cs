namespace Core.Services.UI
{
	public class UIServiceConfiguration : ServiceConfiguration
	{
		public UIContainer MainCanvas;

		public UIScreenBlocker UIScreenBlocker;

		public override Service ServiceClass => new UIService(this);
	}
}