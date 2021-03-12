// using DG.Tweening;

using UnityEngine.UI;

namespace Core.Tools.UI
{
    public class UIScreenBlocker : UIElement
    {
        public bool IsBlockingScreen { get; private set; } = false;

        private Image _image;

        protected override void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
            _UiType = UIType.ScreenBlocker;
        }

        protected override void Start()
        {
            // _image.DOFade(0, 0);
            _image.raycastTarget = false;
        }

        public void BlockScreen(bool isblock)
        {
            IsBlockingScreen = isblock;
            if (isblock)
            {
                _image.raycastTarget = true;
                Show();
            }
            else
            {
                Hide();
                _image.raycastTarget = false;
            }
        }

        protected override void OnElementHide() { }

        protected override void OnElementShow() { }
    }
}