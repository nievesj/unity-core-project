using UnityEngine;
using UnityEngine.UI;

namespace Core.Services.UI
{
    /// <summary>
    /// Represents a sliding panel that is always present on the UI but not necessarily showing or open.
    /// </summary>
    public class UIPanel : UIElement
    {
        [SerializeField]
        protected Button _showHideButton;

        protected bool isShowing = false;
        protected bool isTransitioning = false;

        protected override void Awake()
        {
            base.Awake();

            _UiType = UIType.Panel;

            if (_showHideButton)
                _showHideButton.onClick.AddListener(OnShowHideButtonClick);
        }

        public void OnShowHideButtonClick()
        {
            if (!isTransitioning)
            {
                isTransitioning = true;

                if (isShowing)
                    Hide();
                else
                    Show();
            }
        }

        protected override void OnElementShow()
        {
            isShowing = true;
            isTransitioning = false;
        }

        protected override void OnElementHide()
        {
            isShowing = false;
            isTransitioning = false;
        }

        protected override void OnDestroy()
        {
            Close();
        }
    }
}