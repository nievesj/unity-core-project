using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Services.UI
{
    /// <summary>
    /// Dialog UI is a window or screen that expects some kind of user input.
    /// IUIService sends OnGamePause signal when a window of this type is opened.
    /// </summary>
    public class UIDialog : UIElement
    {
        [SerializeField]
        protected Button _closeButton;

        protected override void Awake()
        {
            base.Awake();
            _UiType = UIType.Dialog;

            if (_closeButton)
                _closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        public void OnCloseButtonClicked()
        {
            Close().Run();
        }

        protected override void OnElementShow() { }

        protected override void OnElementHide() { }
    }
}