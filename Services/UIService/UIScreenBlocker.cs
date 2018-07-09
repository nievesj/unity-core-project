using System;
using System.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;

namespace Core.Services.UI
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
            base.Start();
            
            _image.DOFade(1, 0);
            _image.raycastTarget = false;
        }

        public async Task BlockScreen(bool isblock)
        {
            IsBlockingScreen = isblock;
            if (isblock)
            {
                _image.raycastTarget = true;
                await Show();
            }
            else
            {
                await Hide();
                _image.raycastTarget = false;
            }
        }

        protected override void OnElementHide() { }

        protected override void OnElementShow() { }
    }
}