namespace Core.Services.UI
{
    /// <summary>
    /// Widget UI is used to display bits of information on screen.
    /// Can be used to display player health, currency, tooltips, etc.
    /// </summary>
    public class UIWidget : UIElement
    {
        protected bool IsShowing = false;
        protected bool IsTransitioning = false;

        protected override void Awake()
        {
            base.Awake();
            _UiType = UIType.Widget;
        }

        public void ShowHideWidget()
        {
            if (!IsTransitioning)
            {
                IsTransitioning = true;

                if (IsShowing)
                    Hide().Run();
                else
                    Show().Run();
            }
        }

        protected override void OnElementShow()
        {
            IsShowing = true;
            IsTransitioning = false;
        }

        protected override void OnElementHide()
        {
            IsShowing = false;
            IsTransitioning = false;
        }
    }
}